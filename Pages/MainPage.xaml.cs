using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using Ona_Core;
using Sheas_Cealer_Droid.Consts;
using Sheas_Cealer_Droid.Models;
using Sheas_Cealer_Droid.Preses;
using Sheas_Cealer_Droid.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
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

    private string KaomojiOriginalString = string.Empty;
    private int KaomojiRunningNum = 0;

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

            if (!File.Exists(MainConst.UpstreamHostPath))
                await File.Create(MainConst.UpstreamHostPath).DisposeAsync();
            if (!File.Exists(MainConst.LocalHostPath))
                await File.Create(MainConst.LocalHostPath).DisposeAsync();

            foreach (string cealHostPath in Directory.GetFiles(CealHostWatcher.Path, CealHostWatcher.Filter))
                CealHostWatcher_Changed(null!, new(new(), Path.GetDirectoryName(cealHostPath)!, Path.GetFileName(cealHostPath)));

            if (MainPres.IsUpdateHostEnabled)
                UpdateHostToolbarItem_Clicked(null, null!);

            if (MainPres.IsUpdateSoftwareEnabled)
                UpdateSoftwareToolbarItem_Clicked(null, null!);

            MainPres.IsPageLoading = false;
        });
    }

    private async void LaunchButton_Click(object sender, EventArgs e)
    {
        HapticFeedback.Default.Perform(HapticFeedbackType.Click);

        if (MainPres.IsCommandLineUtd == true)
            await File.WriteAllTextAsync(GlobalConst.CommandLinePath, string.Empty);
        else if (MainPres.IsCommandLineUtd == null)
            await File.WriteAllTextAsync(GlobalConst.CommandLinePath, $"{MainPres.BrowserName!.ToLowerInvariant()} {CealArgs}");
        else
            await File.WriteAllTextAsync(MainConst.UpstreamHostPath, LatestUpstreamHostString);

        await StatusManager.RefreshCurrentStatus(MainPres, CealHostRulesDict.ContainsValue(null));
    }
    [SuppressMessage("Performance", "CA1869"), SuppressMessage("CodeQuality", "IDE0079")]
    private async void AddButton_Clicked(object sender, EventArgs e)
    {
        if (MainPres.IsHostCollectionAtBottom)
        {
            MainCollectionView.ScrollTo(0);

            return;
        }

        string? customHost = (await DisplayPromptAsync(MainConst._AddHostPopupTitle, MainConst._AddHostPopupMsg, GlobalConst._PopupAcceptText, GlobalConst._PopupCancelText))?.Trim().TrimEnd(',');

        if (string.IsNullOrWhiteSpace(customHost))
            return;

        JsonDocumentOptions customHostOptions = new() { AllowTrailingCommas = true };
        JsonElement customHostRule;

        try { customHostRule = JsonDocument.Parse(customHost, customHostOptions).RootElement; }
        catch
        {
            await Toast.Make(MainConst._CustomHostSyntaxErrorToastMsg, ToastDuration.Long).Show();

            return;
        }

        bool isCustomHostValid = true;

        if (customHostRule.ValueKind != JsonValueKind.Array ||
            customHostRule.EnumerateArray().Count() != 3 ||
            customHostRule[0].ValueKind != JsonValueKind.Array ||
            !customHostRule[0].EnumerateArray().Any() ||
            customHostRule[1].ValueKind != JsonValueKind.String && customHostRule[1].ValueKind != JsonValueKind.Null ||
            customHostRule[2].ValueKind != JsonValueKind.String)
            isCustomHostValid = false;

        if (isCustomHostValid)
            foreach (JsonElement customHostDomain in customHostRule[0].EnumerateArray())
                if (customHostDomain.ValueKind != JsonValueKind.String)
                {
                    isCustomHostValid = false;

                    break;
                }
                else if (string.IsNullOrEmpty(customHostDomain.ToString().TrimStart('#').TrimStart('$')))
                {
                    await Toast.Make(MainConst._CustomHostEmptyErrorToastMsg, ToastDuration.Long).Show();

                    return;
                }

        if (!isCustomHostValid)
        {
            await Toast.Make(MainConst._CustomHostFormatErrorToastMsg, ToastDuration.Long).Show();

            return;
        }

        string localHost = await File.ReadAllTextAsync(MainConst.LocalHostPath);

        if (string.IsNullOrWhiteSpace(localHost))
            localHost = "[]";

        JsonDocumentOptions localHostOptions = new() { AllowTrailingCommas = true, CommentHandling = JsonCommentHandling.Skip };
        JsonElement localHostArray = JsonDocument.Parse(localHost, localHostOptions).RootElement;

        List<JsonElement> newHostList = [.. localHostArray.EnumerateArray(), customHostRule];
        JsonSerializerOptions newHostOptions = new();
        string newHost = JsonSerializer.Serialize(newHostList, newHostOptions);

        await File.WriteAllTextAsync(MainConst.LocalHostPath, newHost);
    }

    private void MainSearchHandler_ItemSelected(object _, CealHostRule e) => MainCollectionView.ScrollTo(e, position: ScrollToPosition.Center);

    private void GithubToolbarItem_Clicked(object sender, EventArgs e) => Browser.Default.OpenAsync(MainConst.GithubRepoUrl);
    private async void UpdateHostToolbarItem_Clicked(object? sender, EventArgs e)
    {
        string upstreamUrl = string.IsNullOrEmpty(MainPres.UpstreamUrl) ? GlobalConst.DefaultUpstreamUrl : MainPres.UpstreamUrl;

        string[] latestUpstreamHostUrlArray = MainPres.IsUpstreamMirrorEnabled ?
            upstreamUrl.Contains("github.com", StringComparison.OrdinalIgnoreCase) ?
                [upstreamUrl, upstreamUrl.Replace("github.com", "gitlab.com", StringComparison.OrdinalIgnoreCase), MainConst.GithubMirrorUrl + upstreamUrl] :
            upstreamUrl.Contains("gitlab.com", StringComparison.OrdinalIgnoreCase) ?
                [upstreamUrl, upstreamUrl.Replace("gitlab.com", "github.com", StringComparison.OrdinalIgnoreCase),
                MainConst.GithubMirrorUrl + upstreamUrl.Replace("gitlab.com", "github.com", StringComparison.OrdinalIgnoreCase)] :
            [upstreamUrl] : [upstreamUrl];

        int latestUpstreamHostUrlArrayIndex = 0;

        while (true)
            try
            {
                await StatusProgressBar.ProgressTo(0, 0, Easing.Linear);
                await StatusProgressBar.ProgressTo(0.2, 100, Easing.CubicIn);

                LatestUpstreamHostString = await Http.GetAsync<string>(latestUpstreamHostUrlArray[latestUpstreamHostUrlArrayIndex], MainClient);
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
            catch when (latestUpstreamHostUrlArrayIndex < latestUpstreamHostUrlArray.Length - 1)
            {
                latestUpstreamHostUrlArrayIndex++;

                await Task.Delay(TimeSpan.FromSeconds(0.1));
            }
            catch when (sender == null)
            {
                latestUpstreamHostUrlArrayIndex = 0;

                await Task.Delay(TimeSpan.FromSeconds(1));
            }
            catch
            {
                MainPres.StatusProgress = 1;

                throw;
            }
    }
    private async void UpdateSoftwareToolbarItem_Clicked(object? sender, EventArgs e)
    {
        do
        {
            try
            {
                if (sender != null)
                {
                    await StatusProgressBar.ProgressTo(0, 0, Easing.Linear);
                    await StatusProgressBar.ProgressTo(0.2, 100, Easing.CubicIn);
                }

                MainClient.DefaultRequestHeaders.Add("User-Agent", MainConst.UpdateApiUserAgent);

                JsonElement updateInfoObject = JsonDocument.Parse(await Http.GetAsync<string>(MainConst.UpdateApiUrl, MainClient)).RootElement;

                MainClient.DefaultRequestHeaders.Clear();

                foreach (JsonProperty updateInfoContent in updateInfoObject.EnumerateObject())
                    if (updateInfoContent.Name == "name" && updateInfoContent.Value.ToString() != GlobalConst.VersionAboutInfoContent)
                        if (await DisplayAlert(MainConst._UpdateAvailablePopupTitle, MainConst._UpdateAvailablePopupMsg, GlobalConst._PopupYesText, GlobalConst._PopupNoText))
                            await Browser.Default.OpenAsync(MainConst.GithubReleaseUrl);

                if (sender != null)
                    MainPres.StatusProgress = 1;

                return;
            }
            catch when (sender == null) { await Task.Delay(TimeSpan.FromSeconds(1)); }
            catch
            {
                MainPres.StatusProgress = 1;

                throw;
            }
        } while (sender == null);
    }

    private void MainCollectionView_Scrolled(object sender, ItemsViewScrolledEventArgs e) => MainPres.IsHostCollectionAtBottom = e.LastVisibleItemIndex == MainPres.CealHostRulesCollection.Count - 1;

    private async void BottomTapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        if (KaomojiRunningNum == 0)
            KaomojiOriginalString = MainPres.StatusMessage;

        Interlocked.Add(ref KaomojiRunningNum, 1);

        int kaomojiArrayMaxIndex = MainConst.KaomojiShakeArray.Length - 1;
        int kaomojiArrayIndex = 0;

        Timer kaomojiAnimationTimer = new(_ => MainPres.StatusMessage = KaomojiRunningNum + new string(' ', 2 * KaomojiRunningNum % 60) + MainConst.KaomojiShakeArray[^(Math.Abs(kaomojiArrayIndex++ % (2 * kaomojiArrayMaxIndex) - kaomojiArrayMaxIndex) + 1)],
            null, 0, Math.Max(100 - 2 * KaomojiRunningNum, 1));

        await Task.Delay(400 * kaomojiArrayMaxIndex);
        await kaomojiAnimationTimer.DisposeAsync();

        Interlocked.Add(ref KaomojiRunningNum, -1);

        if (KaomojiRunningNum == 0)
            MainPres.StatusMessage = KaomojiOriginalString;
    }

    private void LayoutSwipeGestureRecognizer_Swiped(object sender, SwipedEventArgs e) => Shell.Current.FlyoutIsPresented = true;

    private async void CealHostWatcher_Changed(object sender, FileSystemEventArgs e)
    {
        string cealHostName = e.Name!.TrimStart("Cealing-Host-".ToCharArray()).TrimEnd(".json".ToCharArray());

        try
        {
            CealHostRulesDict[cealHostName] = [];

            string cealHost = await File.ReadAllTextAsync(e.FullPath);

            if (cealHost.Length == 0)
                return;

            JsonDocumentOptions cealHostOptions = new() { AllowTrailingCommas = true, CommentHandling = JsonCommentHandling.Skip };
            JsonElement cealHostArray = JsonDocument.Parse(cealHost, cealHostOptions).RootElement;

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

            if (!string.IsNullOrWhiteSpace(await File.ReadAllTextAsync(GlobalConst.CommandLinePath)))
            {
                await File.WriteAllTextAsync(GlobalConst.CommandLinePath, $"{MainPres.BrowserName!.ToLowerInvariant()} {CealArgs}");

                await StatusManager.RefreshCurrentStatus(MainPres, CealHostRulesDict.ContainsValue(null));
            }
        }
    }
}