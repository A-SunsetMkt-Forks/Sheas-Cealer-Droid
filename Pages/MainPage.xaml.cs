using Microsoft.Maui;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Ona_Core;
using Sheas_Cealer_Droid.Consts;
using Sheas_Cealer_Droid.Models;
using Sheas_Cealer_Droid.Preses;
using Sheas_Cealer_Droid.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sheas_Cealer_Droid.Pages;

public partial class MainPage : ContentPage
{
    private readonly MainPres MainPres;
    private readonly HttpClient MainClient = new(new HttpClientHandler { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator });
    private readonly FileSystemWatcher CealHostWatcher = new(Path.GetDirectoryName(MainConst.CealHostPath)!, Path.GetFileName(MainConst.CealHostPath)) { EnableRaisingEvents = true, NotifyFilter = NotifyFilters.LastWrite };

    private readonly SortedDictionary<string, List<(List<(string includeDomain, string excludeDomain)> domainPairs, string? sni, string ip)>?> CealHostRulesDict = [];
    private string CealArgs = string.Empty;
    private string LatestUpstreamHostString = string.Empty;

    public MainPage()
    {
        InitializeComponent();

        BindingContext = MainPres = new();
    }
    private async void MainWin_Loaded(object sender, EventArgs e)
    {
        await Task.Run(async () =>
        {
            CealHostWatcher.Changed += CealHostWatcher_Changed;

            if (!File.Exists(MainConst.CommandLinePath))
                await File.Create(MainConst.CommandLinePath).DisposeAsync();
            if (!File.Exists(MainConst.UpstreamHostPath))
                await File.Create(MainConst.UpstreamHostPath).DisposeAsync();

            foreach (string cealHostPath in Directory.GetFiles(CealHostWatcher.Path, CealHostWatcher.Filter))
                CealHostWatcher_Changed(null!, new(new(), Path.GetDirectoryName(cealHostPath)!, Path.GetFileName(cealHostPath)));

            UpdateToolbarItem_Clicked(null, null!);

            MainPres.IsPageLoading = false;
        });
    }

    private async void LaunchButton_Click(object sender, EventArgs e)
    {
        if (MainPres.IsCommandLineUtd == true)
            await File.WriteAllTextAsync(MainConst.CommandLinePath, string.Empty);
        else if (MainPres.IsCommandLineUtd == null)
            await File.WriteAllTextAsync(MainConst.CommandLinePath, $"{MainPres.BrowserName!.ToLowerInvariant()} {CealArgs}");
        else
            await File.WriteAllTextAsync(MainConst.UpstreamHostPath, LatestUpstreamHostString);

        await StatusManager.RefreshCurrentStatus(MainPres, CealHostRulesDict.ContainsValue(null));
    }

    private void MainSearchHandler_ItemSelected(object _, CealHostRule e) => MainCollectionView.ScrollTo(e, position: ScrollToPosition.Center);

    private void GithubToolbarItem_Clicked(object sender, EventArgs e) => Browser.Default.OpenAsync(MainConst.GithubRepoUrl);
    private async void UpdateToolbarItem_Clicked(object? sender, EventArgs e)
    {
        do
        {
            try
            {
                await StatusProgressBar.ProgressTo(0, 0, Easing.Linear);
                await StatusProgressBar.ProgressTo(0.2, 100, Easing.CubicIn);

                LatestUpstreamHostString = await Http.GetAsync<string>(MainConst.DefaultUpstreamUrl, MainClient);
                string localUpstreamHostString = await File.ReadAllTextAsync(MainConst.UpstreamHostPath);

                try { LatestUpstreamHostString = Encoding.UTF8.GetString(Convert.FromBase64String(LatestUpstreamHostString)); }
                catch { }

                if (localUpstreamHostString != LatestUpstreamHostString && localUpstreamHostString.ReplaceLineEndings() != LatestUpstreamHostString.ReplaceLineEndings())
                {
                    MainPres.IsCommandLineUtd = false;
                    MainPres.StatusMessage = MainConst._UpdateStatusMessage;
                }

                MainPres.StatusProgress = 1;

                return;
            }
            catch when (sender == null) { }
            catch
            {
                MainPres.StatusProgress = 1;

                throw;
            }
        } while (sender == null);
    }

    private async void CealHostWatcher_Changed(object sender, FileSystemEventArgs e)
    {
        string cealHostName = e.Name!.TrimStart("Cealing-Host-".ToCharArray()).TrimEnd(".json".ToCharArray());

        try
        {
            CealHostRulesDict[cealHostName] = [];

            await using FileStream cealHostStream = new(e.FullPath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);

            if (cealHostStream.Length == 0)
                return;

            JsonDocumentOptions cealHostOptions = new() { AllowTrailingCommas = true, CommentHandling = JsonCommentHandling.Skip };
            JsonElement cealHostArray = (await JsonDocument.ParseAsync(cealHostStream, cealHostOptions)).RootElement;

            foreach (JsonElement cealHostRule in cealHostArray.EnumerateArray())
            {
                List<(string includeDomain, string excludeDomain)> cealHostDomainPairs = [];
                string? cealHostSni = cealHostRule[1].ValueKind == JsonValueKind.Null ? null :
                    string.IsNullOrWhiteSpace(cealHostRule[1].ToString()) ? $"{cealHostName}{CealHostRulesDict[cealHostName]!.Count}" : cealHostRule[1].ToString().Trim();
                string cealHostIp = string.IsNullOrWhiteSpace(cealHostRule[2].ToString()) ? "127.0.0.1" : cealHostRule[2].ToString().Trim();

                foreach (JsonElement cealHostDomain in cealHostRule[0].EnumerateArray())
                {
                    string[] cealHostDomainPair = cealHostDomain.ToString().Split('^', 2, StringSplitOptions.TrimEntries);

                    if (string.IsNullOrEmpty(cealHostDomainPair[0].TrimStart('#').TrimStart('$')))
                        continue;

                    cealHostDomainPairs.Add((cealHostDomainPair[0], cealHostDomainPair.Length == 2 ? cealHostDomainPair[1] : string.Empty));
                }

                if (cealHostDomainPairs.Count != 0)
                {
                    CealHostRulesDict[cealHostName]!.Add((cealHostDomainPairs, cealHostSni, cealHostIp));
                    MainPres.CealHostRulesCollection.Add(new()
                    {
                        Name = cealHostName,
                        Domain = cealHostRule[0].ToString().TrimStart('[').TrimEnd(']').Replace("\"", string.Empty),
                        Sni = string.IsNullOrWhiteSpace(cealHostRule[1].ToString()) ? "--" : cealHostRule[1].ToString(),
                        Ip = cealHostIp
                    });
                }
            }
        }
        catch { CealHostRulesDict[cealHostName] = null; }
        finally
        {
            string hostRules = string.Empty;
            string hostResolverRules = string.Empty;
            int nullSniNum = 0;

            foreach (KeyValuePair<string, List<(List<(string includeDomain, string excludeDomain)> domainPairs, string? sni, string ip)>?> cealHostRulesPair in CealHostRulesDict)
                foreach ((List<(string includeDomain, string excludeDomain)> cealHostDomainPairs, string? cealHostSni, string cealHostIp) in cealHostRulesPair.Value ?? [])
                {
                    string cealHostSniWithoutNull = cealHostSni ?? $"{cealHostRulesPair.Key}{(cealHostRulesPair.Value ?? []).Count + ++nullSniNum}";
                    bool isValidCealHostDomainExist = false;

                    foreach ((string cealHostIncludeDomain, string cealHostExcludeDomain) in cealHostDomainPairs)
                    {
                        if (cealHostIncludeDomain.StartsWith('$'))
                            continue;

                        hostRules += $"MAP {cealHostIncludeDomain.TrimStart('#')} {cealHostSniWithoutNull}," + (!string.IsNullOrWhiteSpace(cealHostExcludeDomain) ? $"EXCLUDE {cealHostExcludeDomain}," : string.Empty);
                        isValidCealHostDomainExist = true;
                    }

                    if (isValidCealHostDomainExist)
                        hostResolverRules += $"MAP {cealHostSniWithoutNull} {cealHostIp},";
                }

            CealArgs = @$"--host-rules=""{hostRules.TrimEnd(',')}"" --host-resolver-rules=""{hostResolverRules.TrimEnd(',')}"" --test-type --ignore-certificate-errors";

            if (!string.IsNullOrWhiteSpace(await File.ReadAllTextAsync(MainConst.CommandLinePath)))
            {
                await File.WriteAllTextAsync(MainConst.CommandLinePath, $"{MainPres.BrowserName!.ToLowerInvariant()} {CealArgs}");

                await StatusManager.RefreshCurrentStatus(MainPres, CealHostRulesDict.ContainsValue(null));
            }
        }
    }

    private void LayoutSwipeGestureRecognizer_Swiped(object sender, SwipedEventArgs e) => Shell.Current.FlyoutIsPresented = true;
}