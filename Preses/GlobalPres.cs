using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Sheas_Cealer_Droid.Consts;
using Sheas_Cealer_Droid.Models;
using Sheas_Cealer_Droid.Utils;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Sheas_Cealer_Droid.Preses;

internal abstract partial class GlobalPres : ObservableObject
{
    protected GlobalPres()
    {
        BrowserNameCollection.CollectionChanged += BrowserNameCollection_CollectionChanged;

        themeColorName = Preferences.Default.ContainsKey(nameof(ThemeColorName)) ?
            GlobalConst.ResourceManager.GetString(Preferences.Default.Get(nameof(ThemeColorName), string.Empty))! : GlobalConst._ThemeColorRedName;

        themeStateName = Preferences.Default.ContainsKey(nameof(ThemeStateName)) ?
            GlobalConst.ResourceManager.GetString(Preferences.Default.Get(nameof(ThemeStateName), string.Empty))! : GlobalConst._ThemeStateUnspecifiedName;

        langOptionName = Preferences.Default.ContainsKey(nameof(LangOptionName)) ?
            GlobalConst.ResourceManager.GetString(Preferences.Default.Get(nameof(LangOptionName), string.Empty))! : GlobalConst._LangOptionUnspecifiedName;
    }

    [ObservableProperty]
    private static ObservableCollection<CealHostRule> cealHostRulesCollection = [];

    private static readonly ObservableCollection<string> browserNameCollection = (Preferences.Default.ContainsKey(nameof(BrowserNameCollection)) ? JsonSerializer.Deserialize<ObservableCollection<string>>(Preferences.Default.Get(nameof(BrowserNameCollection), string.Empty)) : GlobalConst.DefaultBrowserNameCollection)!;
    [SuppressMessage("Performance", "CA1822"), SuppressMessage("CodeQuality", "IDE0079")]
    public ObservableCollection<string> BrowserNameCollection => browserNameCollection;
    private void BrowserNameCollection_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) => Preferences.Default.Set(nameof(BrowserNameCollection), JsonSerializer.Serialize(BrowserNameCollection));

    [ObservableProperty]
    private static string? browserName = Preferences.Default.Get<string?>(nameof(BrowserName), null);
    async partial void OnBrowserNameChanged(string? oldValue, string? newValue)
    {
        if (!IsFirstRunning && string.IsNullOrEmpty(newValue))
        {
            BrowserName = oldValue;

            return;
        }

        if (newValue == BrowserNameCollection[^1])
        {
            string? customBrowserName = await Shell.Current.CurrentPage.DisplayPromptAsync(GlobalConst._CustomBrowserNamePopupTitle, GlobalConst._CustomBrowserNamePopupMsg, GlobalConst._PopupAcceptText, GlobalConst._PopupCancelText);

            if (string.IsNullOrEmpty(customBrowserName) || customBrowserName.Any(c => char.IsControl(c) || char.IsWhiteSpace(c)))
            {
                if (customBrowserName != null)
                    await Toast.Make(GlobalConst._CustomBrowserNameErrorToastMsg, ToastDuration.Long).Show();

                BrowserName = oldValue;

                return;
            }

            BrowserNameCollection.Insert(BrowserNameCollection.Count - 1, customBrowserName);
            newValue = customBrowserName;
        }
        else if (!GlobalConst.DefaultBrowserNameCollection.Contains(newValue!) && oldValue != BrowserNameCollection[^1])
            if (!await Shell.Current.CurrentPage.DisplayAlert(GlobalConst._CustomBrowserNameApplyPopupTitle, GlobalConst._CustomBrowserNameApplyPopupMsg, GlobalConst._PopupApplyText, GlobalConst._PopupDeleteText))
            {
                BrowserName = BrowserNameCollection[0];
                BrowserNameCollection.Remove(newValue!);
                newValue = BrowserNameCollection[0];
            }

        if (IsCommandLineUtd == true)
            await CommandLineWriter.Write(BrowserName!, App.CealArgs, ExtraArgs.Trim());

        Preferences.Default.Set(nameof(BrowserName), newValue);
    }

    [ObservableProperty]
    private static string upstreamUrl = Preferences.Default.Get(nameof(UpstreamUrl), string.Empty);
    partial void OnUpstreamUrlChanged(string value)
    {
        IsUpstreamMirrorEnabled = string.IsNullOrEmpty(value) || value.Contains("github.com", StringComparison.OrdinalIgnoreCase) || value.Contains("gitlab.com", StringComparison.OrdinalIgnoreCase);

        Preferences.Default.Set(nameof(UpstreamUrl), value);
    }

    [ObservableProperty]
    private static string extraArgs = Preferences.Default.Get(nameof(ExtraArgs), string.Empty);
    async partial void OnExtraArgsChanged(string value)
    {
        if (IsCommandLineUtd == true)
            await CommandLineWriter.Write(BrowserName!, App.CealArgs, ExtraArgs.Trim());

        Preferences.Default.Set(nameof(ExtraArgs), value);
    }

    [ObservableProperty]
    private static string themeColorName = string.Empty;
    async partial void OnThemeColorNameChanged(string? oldValue, string newValue)
    {
        if (newValue == GlobalConst._ThemeColorPinkBlueName)
        {
            string userPairCode = BitConverter.ToString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(DateTime.UtcNow.ToString("yyyyMMddHH") + GlobalConst.UserPairIdentity))).Replace("-", string.Empty)[..4].ToUpperInvariant();
            string? inputPairCode = (await Shell.Current.CurrentPage.DisplayPromptAsync(GlobalConst._DualThemeColorPairPopupTitle, string.Format(GlobalConst._DualThemeColorPairPopupMsg, userPairCode), GlobalConst._PopupAcceptText, GlobalConst._PopupCancelText))?.ToUpperInvariant();
            bool isPairCodeMatched = false;

            if (string.IsNullOrWhiteSpace(inputPairCode))
            {
                ThemeColorName = oldValue!;

                await Toast.Make(GlobalConst._DualThemeColorPairCancelToastMsg).Show();

                return;
            }

            if (inputPairCode == userPairCode)
            {
                ThemeColorName = oldValue!;

                await Toast.Make(GlobalConst._DualThemeColorPairRepeatToastMsg).Show();

                return;
            }

            for (int pairIdentity = 0; pairIdentity < 10; pairIdentity++)
            {
                if (pairIdentity == GlobalConst.UserPairIdentity)
                    continue;

                if (BitConverter.ToString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(DateTime.UtcNow.ToString("yyyyMMddHH") + pairIdentity))).Replace("-", string.Empty)[..4].ToUpperInvariant() == inputPairCode)
                {
                    isPairCodeMatched = true;

                    break;
                }
            }

            if (!isPairCodeMatched)
            {
                ThemeColorName = oldValue!;

                await Toast.Make(GlobalConst._DualThemeColorPairErrorToastMsg).Show();

                return;
            }

            await Toast.Make(GlobalConst._DualThemeColorPairSuccessToastMsg).Show();
        }

        Preferences.Default.Set(nameof(ThemeColorName), ResourceKeyFinder.FindGlobalKey(newValue));

        if (oldValue != GlobalConst._ThemeColorPinkBlueName && newValue != GlobalConst._ThemeColorPinkBlueName)
            await Toast.Make(GlobalConst._ThemeColorRestartToApplyToastMsg).Show();
    }

    [ObservableProperty]
    private static string themeStateName = string.Empty;
    async partial void OnThemeStateNameChanged(string value)
    {
        Preferences.Default.Set(nameof(ThemeStateName), ResourceKeyFinder.FindGlobalKey(value));

        await Toast.Make(GlobalConst._ThemeStateRestartToApplyToastMsg).Show();
    }

    [ObservableProperty]
    private static string langOptionName = string.Empty;
    async partial void OnLangOptionNameChanged(string value)
    {
        Preferences.Default.Set(nameof(LangOptionName), ResourceKeyFinder.FindGlobalKey(value));

        await Toast.Make(GlobalConst._LangOptionRestartToApplyToastMsg).Show();
    }

    [ObservableProperty]
    private static bool isUpstreamMirrorEnabled = Preferences.Default.Get(nameof(IsUpstreamMirrorEnabled), string.IsNullOrEmpty(upstreamUrl) || upstreamUrl.Contains("github.com", StringComparison.OrdinalIgnoreCase) || upstreamUrl.Contains("gitlab.com", StringComparison.OrdinalIgnoreCase));
    partial void OnIsUpstreamMirrorEnabledChanged(bool value) => Preferences.Default.Set(nameof(IsUpstreamMirrorEnabled), value);

    [ObservableProperty]
    private static bool isUpdateSoftwareEnabled = Preferences.Default.Get(nameof(IsUpdateSoftwareEnabled), true);
    partial void OnIsUpdateSoftwareEnabledChanged(bool value) => Preferences.Default.Set(nameof(IsUpdateSoftwareEnabled), value);

    [ObservableProperty]
    private static bool isUpdateHostEnabled = Preferences.Default.Get(nameof(IsUpdateHostEnabled), true);
    partial void OnIsUpdateHostEnabledChanged(bool value) => Preferences.Default.Set(nameof(IsUpdateHostEnabled), value);

    [ObservableProperty]
    private static bool isCopyEnabled = Preferences.Default.Get(nameof(IsCopyEnabled), true);
    async partial void OnIsCopyEnabledChanged(bool value)
    {
        Preferences.Default.Set(nameof(IsCopyEnabled), value);

        await Toast.Make(GlobalConst._SettingsRestartToApplyToastMsg).Show();
    }

    [ObservableProperty]
    private static bool isDelayEnabled = Preferences.Default.Get(nameof(IsDelayEnabled), true);
    async partial void OnIsDelayEnabledChanged(bool value)
    {
        Preferences.Default.Set(nameof(IsDelayEnabled), value);

        await Toast.Make(GlobalConst._SettingsRestartToApplyToastMsg).Show();
    }

    [ObservableProperty]
    private static bool isSearchEnabled = Preferences.Default.Get(nameof(IsSearchEnabled), true);
    async partial void OnIsSearchEnabledChanged(bool value)
    {
        Preferences.Default.Set(nameof(IsSearchEnabled), value);

        await Toast.Make(GlobalConst._SettingsRestartToApplyToastMsg).Show();
    }

    [ObservableProperty]
    private static bool isFlagCopied = Preferences.Default.Get(nameof(IsFlagCopied), false);
    partial void OnIsFlagCopiedChanged(bool value) => Preferences.Default.Set(nameof(IsFlagCopied), value);

    [ObservableProperty]
    private static bool isCommandLineExist = Preferences.Default.Get(nameof(IsCommandLineExist), false);
    partial void OnIsCommandLineExistChanged(bool value) => Preferences.Default.Set(nameof(IsCommandLineExist), value);

    [ObservableProperty]
    private static bool? isCommandLineUtd = null;

    [ObservableProperty]
    private static bool isFirstRunning = Preferences.Default.Get(nameof(IsFirstRunning), true);
    partial void OnIsFirstRunningChanged(bool value)
    {
        Preferences.Default.Set(nameof(GlobalConst.UserPairIdentity), GlobalConst.UserPairIdentity);
        Preferences.Default.Set(nameof(IsFirstRunning), value);
    }
}