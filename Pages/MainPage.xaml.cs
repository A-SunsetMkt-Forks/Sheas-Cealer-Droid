using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using Ona_Core;
using Sheas_Cealer_Droid.Anims;
using Sheas_Cealer_Droid.Consts;
using Sheas_Cealer_Droid.Models;
using Sheas_Cealer_Droid.Preses;
using Sheas_Cealer_Droid.Utils;
using System;
using System.Collections.Generic;
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

    private readonly SortedDictionary<string, List<CealHostRule>?> CealHostRulesDict = [];
    private string CealArgs = string.Empty;
    private string LatestUpstreamHostString = string.Empty;

    private readonly SemaphoreSlim IsCealHostChangingSemaphore = new(1);
    private bool IsAddImageButtonSlideAnimRunning = false;

    private string KaomojiOriginalString = string.Empty;
    private int KaomojiRunningCount = 0;

    public MainPage()
    {
        InitializeComponent();

        BindingContext = MainPres = new();
    }
    private async void MainWin_Loaded(object sender, EventArgs e)
    {
        await Task.Run(async () =>
        {
            AddImageButton.TranslationY = 60;
            new AddImageButtonSlideAnim(AddImageButton, AddImageButtonSlideAnim.SlideType.In).Commit(this, nameof(AddImageButton) + nameof(AddImageButtonSlideAnim), 8, 1000);

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

    private async void LaunchImageButton_Click(object sender, EventArgs e)
    {
        HapticFeedback.Default.Perform(HapticFeedbackType.Click);

        if (MainPres.IsCommandLineUtd == true)
            await File.WriteAllTextAsync(GlobalConst.CommandLinePath, string.Empty);
        else if (MainPres.IsCommandLineUtd == null)
        {
            string cealArgs = MainPres.ExtraArgs.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Contains(MainConst.DisableCealArg) ? string.Empty : CealArgs + " ";

            await File.WriteAllTextAsync(GlobalConst.CommandLinePath, $"{MainPres.BrowserName!.ToLowerInvariant()} {cealArgs}{MainPres.ExtraArgs.TrimStart()}".TrimEnd());
        }
        else
            await File.WriteAllTextAsync(MainConst.UpstreamHostPath, LatestUpstreamHostString);

        await StatusManager.RefreshCurrentStatus(MainPres, CealHostRulesDict.ContainsValue(null));
    }
    private async void AddImageButton_Clicked(object sender, EventArgs e)
    {
        if (MainPres.IsHostCollectionAtBottom)
        {
            MainCollectionView.ScrollTo(0);

            return;
        }

        string? customHost = (await DisplayPromptAsync(MainConst._AddHostPopupTitle, MainConst._AddHostPopupMsg, GlobalConst._PopupAcceptText, GlobalConst._PopupCancelText))?.Trim().TrimEnd(',');

        if (string.IsNullOrWhiteSpace(customHost))
            return;

        JsonElement customHostRule;

        try { customHostRule = JsonDocument.Parse(customHost).RootElement; }
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
                else if (string.IsNullOrEmpty(customHostDomain.GetString()?.Trim().TrimStart('#').TrimStart('$')))
                {
                    await Toast.Make(MainConst._CustomHostEmptyErrorToastMsg, ToastDuration.Long).Show();

                    return;
                }

        if (!isCustomHostValid)
        {
            await Toast.Make(MainConst._CustomHostFormatErrorToastMsg, ToastDuration.Long).Show();

            return;
        }

        string customHostDomains = customHostRule[0].ToString();
        string? customHostSni = customHostRule[1].GetString()?.Trim();
        string customHostIp = string.IsNullOrWhiteSpace(customHostRule[2].GetString()) ? "127.0.0.1" : customHostRule[2].GetString()!.Trim();
        string localHostName = Path.GetFileNameWithoutExtension(MainConst.LocalHostPath).TrimStart("Cealing-Host-".ToCharArray());

        MainPres.CealHostRulesCollection.Insert(0, new
        (
            localHostName,
            string.Join(',', JsonSerializer.Deserialize<string[]>(customHostDomains)!),
            string.IsNullOrEmpty(customHostSni) ? "--" : customHostSni,
            customHostIp
        ));

        await Task.Delay(1000);

        if (!CealHostRulesDict.ContainsKey(localHostName))
            CealHostRulesDict[localHostName] = [];

        CealHostRulesDict[localHostName]!.Insert(0, new(customHostDomains, customHostSni, customHostIp));

        List<object?[]> newHostRuleArray = [];

        foreach (CealHostRule localHostRule in CealHostRulesDict[localHostName]!)
        {
            string[] localHostDomainArray = JsonSerializer.Deserialize<string[]>(localHostRule.Domains)!;

            newHostRuleArray.Add([localHostDomainArray, localHostRule.Sni, localHostRule.Ip]);
        }

        await File.WriteAllTextAsync(MainConst.LocalHostPath, JsonSerializer.Serialize(newHostRuleArray));
    }

    private async void RemoveImageButton_Clicked(object sender, EventArgs e)
    {
        ImageButton senderImageButton = (ImageButton)sender;
        CealHostRule selectedHostRule = (CealHostRule)senderImageButton.BindingContext;
        int selectedHostIndex = MainPres.CealHostRulesCollection.IndexOf(selectedHostRule);

        MainPres.CealHostRulesCollection.RemoveAt(selectedHostIndex);

        await Task.Delay(1000);

        foreach (KeyValuePair<string, List<CealHostRule>?> cealHostRulesPair in CealHostRulesDict)
            if (cealHostRulesPair.Key != selectedHostRule.Name)
                selectedHostIndex -= cealHostRulesPair.Value?.Count ?? 0;
            else
                break;

        CealHostRulesDict[selectedHostRule.Name!]!.RemoveAt(selectedHostIndex);

        List<object?[]> newHostRuleList = [];

        foreach (CealHostRule cealHostRule in CealHostRulesDict[selectedHostRule.Name!]!)
            newHostRuleList.Add([JsonSerializer.Deserialize<string[]>(cealHostRule.Domains)!, cealHostRule.Sni, cealHostRule.Ip]);

        await File.WriteAllTextAsync(MainConst.CealHostPath.Replace("*", selectedHostRule.Name), JsonSerializer.Serialize(newHostRuleList));
    }
    private async void CopyImageButton_Clicked(object sender, EventArgs e)
    {
        ImageButton senderImageButton = (ImageButton)sender;
        CealHostRule selectedHostRule = (CealHostRule)senderImageButton.BindingContext;
        int selectedHostIndex = MainPres.CealHostRulesCollection.IndexOf(selectedHostRule);

        foreach (KeyValuePair<string, List<CealHostRule>?> cealHostRulesPair in CealHostRulesDict)
            if (cealHostRulesPair.Key != selectedHostRule.Name)
                selectedHostIndex -= cealHostRulesPair.Value?.Count ?? 0;
            else
                break;

        CealHostRule cealHostRule = CealHostRulesDict[selectedHostRule.Name!]![selectedHostIndex];

        await Clipboard.Default.SetTextAsync(JsonSerializer.Serialize<object?[]>([JsonSerializer.Deserialize<string[]>(cealHostRule.Domains)!, cealHostRule.Sni, cealHostRule.Ip]));
        await Toast.Make(MainConst._HostCopiedToastMsg).Show();
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
                    if (updateInfoContent.Name == "name" && updateInfoContent.Value.GetString() != GlobalConst.VersionAboutInfoContent)
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

    private void MainCollectionView_Scrolled(object sender, ItemsViewScrolledEventArgs e)
    {
        if (IsAddImageButtonSlideAnimRunning)
            return;

        bool isHostCollectionAtBottom = e.LastVisibleItemIndex == MainPres.CealHostRulesCollection.Count - 1;

        if (MainPres.IsHostCollectionAtBottom != isHostCollectionAtBottom)
        {
            IsAddImageButtonSlideAnimRunning = true;

            new AddImageButtonSlideAnim(AddImageButton, AddImageButtonSlideAnim.SlideType.Out).Commit(this, nameof(AddImageButton) + nameof(AddImageButtonSlideAnim), 8, 300,
                finished: (_, _) =>
                {
                    MainPres.IsHostCollectionAtBottom = isHostCollectionAtBottom;
                    new AddImageButtonSlideAnim(AddImageButton, AddImageButtonSlideAnim.SlideType.In).Commit(this, nameof(AddImageButton) + nameof(AddImageButtonSlideAnim), 8, 300);

                    IsAddImageButtonSlideAnimRunning = false;
                });
        }
    }

    private async void BottomTapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        if (KaomojiRunningCount == 0)
            KaomojiOriginalString = MainPres.StatusMessage;

        Interlocked.Add(ref KaomojiRunningCount, 1);

        int kaomojiArrayMaxIndex = MainConst.KaomojiShakeArray.Length - 1;
        int kaomojiArrayIndex = 0;

        Timer kaomojiAnimationTimer = new
        (
            _ => MainPres.StatusMessage = KaomojiRunningCount +
                new string(' ', 2 * KaomojiRunningCount % 60) +
                MainConst.KaomojiShakeArray[^(Math.Abs(kaomojiArrayIndex++ % (2 * kaomojiArrayMaxIndex) - kaomojiArrayMaxIndex) + 1)],
            null, 0, Math.Max(100 - 2 * KaomojiRunningCount, 1)
        );

        await Task.Delay(400 * kaomojiArrayMaxIndex);
        await kaomojiAnimationTimer.DisposeAsync();

        Interlocked.Add(ref KaomojiRunningCount, -1);

        if (KaomojiRunningCount == 0)
            MainPres.StatusMessage = KaomojiOriginalString;
    }

    private void LayoutSwipeGestureRecognizer_Swiped(object sender, SwipedEventArgs e) => Shell.Current.FlyoutIsPresented = true;

    private async void CealHostWatcher_Changed(object sender, FileSystemEventArgs e)
    {
        string cealHostName = Path.GetFileNameWithoutExtension(e.Name!).TrimStart("Cealing-Host-".ToCharArray());

        try
        {
            await IsCealHostChangingSemaphore.WaitAsync();

            CealHostRulesDict[cealHostName] = [];

            string cealHost = await File.ReadAllTextAsync(e.FullPath);

            if (cealHost.Length == 0)
                return;

            JsonDocumentOptions cealHostOptions = new() { AllowTrailingCommas = true, CommentHandling = JsonCommentHandling.Skip };
            JsonElement cealHostArray = JsonDocument.Parse(cealHost, cealHostOptions).RootElement;

            foreach (JsonElement cealHostRule in cealHostArray.EnumerateArray())
            {
                foreach (JsonElement cealHostDomain in cealHostRule[0].EnumerateArray())
                    if (string.IsNullOrEmpty(cealHostDomain.GetString()?.Trim().TrimStart('#').TrimStart('$')))
                        continue;

                string cealHostDomains = cealHostRule[0].ToString();
                string? cealHostSni = cealHostRule[1].GetString()?.Trim();
                string cealHostIp = string.IsNullOrWhiteSpace(cealHostRule[2].GetString()) ? "127.0.0.1" : cealHostRule[2].GetString()!.Trim();

                CealHostRulesDict[cealHostName]!.Add(new(cealHostDomains, cealHostSni, cealHostIp));
            }
        }
        catch { CealHostRulesDict[cealHostName] = null; }
        finally
        {
            List<CealHostRule> cealHostRulesList = [];
            string hostRules = string.Empty;
            string hostResolverRules = string.Empty;
            int EmptySniIndex = 0;

            foreach (KeyValuePair<string, List<CealHostRule>?> cealHostRulesPair in CealHostRulesDict)
                foreach (CealHostRule cealHostRule in cealHostRulesPair.Value ?? [])
                {
                    string[] cealHostDomainArray = JsonSerializer.Deserialize<string[]>(cealHostRule.Domains)!;
                    string cealHostDomains = string.Empty;
                    string cealHostSniWithoutEmpty = string.IsNullOrEmpty(cealHostRule.Sni) ? $"{cealHostRulesPair.Key}{EmptySniIndex++}" : cealHostRule.Sni;

                    foreach (string cealHostDomain in cealHostDomainArray)
                    {
                        cealHostDomains += cealHostDomain + ',';

                        if (cealHostDomain.StartsWith('$'))
                            continue;

                        string[] cealHostDomainPair = cealHostDomain.Split('^', 2, StringSplitOptions.TrimEntries);

                        hostRules += $"MAP {cealHostDomainPair[0].TrimStart('#')} {cealHostSniWithoutEmpty}," + (!string.IsNullOrEmpty(cealHostDomainPair.ElementAtOrDefault(1)) ? $"EXCLUDE {cealHostDomainPair[1]}," : string.Empty);
                    }

                    cealHostRulesList.Add(new(cealHostRulesPair.Key, cealHostDomains.TrimEnd(','), string.IsNullOrEmpty(cealHostRule.Sni) ? "--" : cealHostRule.Sni, cealHostRule.Ip));

                    if (!string.IsNullOrEmpty(hostResolverRules))
                        hostResolverRules += $"MAP {cealHostSniWithoutEmpty} {cealHostRule.Ip},";
                }

            IsCealHostChangingSemaphore.Release();

            MainPres.CealHostRulesCollection = new(cealHostRulesList);

            CealArgs = @$"--host-rules=""{hostRules.TrimEnd(',')}"" --host-resolver-rules=""{hostResolverRules.TrimEnd(',')}"" --test-type --ignore-certificate-errors";

            if (!string.IsNullOrWhiteSpace(await File.ReadAllTextAsync(GlobalConst.CommandLinePath)))
            {
                await File.WriteAllTextAsync(GlobalConst.CommandLinePath, $"{MainPres.BrowserName!.ToLowerInvariant()} {CealArgs}");

                await StatusManager.RefreshCurrentStatus(MainPres, CealHostRulesDict.ContainsValue(null));
            }
        }
    }
}