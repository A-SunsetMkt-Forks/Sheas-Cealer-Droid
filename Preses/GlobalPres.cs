using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Sheas_Cealer_Droid.Consts;
using Sheas_Cealer_Droid.Models;
using Sheas_Cealer_Droid.Utils;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
    private static string themeColorName = string.Empty;
    async partial void OnThemeColorNameChanged(string value)
    {
        Preferences.Default.Set(nameof(ThemeColorName), ResourceKeyFinder.FindGlobalKey(value));

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
    private static string? browserName = Preferences.Default.Get<string?>(nameof(BrowserName), null);
    async partial void OnBrowserNameChanged(string? oldValue, string? newValue)
    {
        if (newValue == GlobalConst._BrowserNameCollectionCustomTitle)
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

        Preferences.Default.Set(nameof(BrowserName), newValue);
    }

    private static readonly ObservableCollection<string> browserNameCollection = (Preferences.Default.ContainsKey(nameof(BrowserNameCollection)) ? JsonSerializer.Deserialize<ObservableCollection<string>>(Preferences.Default.Get(nameof(BrowserNameCollection), string.Empty)) : GlobalConst.DefaultBrowserNameCollection)!;
    [SuppressMessage("Performance", "CA1822"), SuppressMessage("CodeQuality", "IDE0079")]
    public ObservableCollection<string> BrowserNameCollection => browserNameCollection;
    private void BrowserNameCollection_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) => Preferences.Default.Set(nameof(BrowserNameCollection), JsonSerializer.Serialize(BrowserNameCollection));

    [ObservableProperty]
    private static string upstreamUrl = Preferences.Default.Get(nameof(UpstreamUrl), string.Empty);
    partial void OnUpstreamUrlChanged(string value) => Preferences.Default.Set(nameof(UpstreamUrl), value);

    private static readonly ObservableCollection<CealHostRule> cealHostRulesCollection = [];
    [SuppressMessage("Performance", "CA1822"), SuppressMessage("CodeQuality", "IDE0079")]
    public ObservableCollection<CealHostRule> CealHostRulesCollection => cealHostRulesCollection;

    [ObservableProperty]
    private static bool isUpdateSoftwareEnabled = Preferences.Default.Get(nameof(IsUpdateSoftwareEnabled), true);
    partial void OnIsUpdateSoftwareEnabledChanged(bool value) => Preferences.Default.Set(nameof(IsUpdateSoftwareEnabled), value);

    [ObservableProperty]
    private static bool isUpdateHostEnabled = Preferences.Default.Get(nameof(IsUpdateHostEnabled), true);
    partial void OnIsUpdateHostEnabledChanged(bool value) => Preferences.Default.Set(nameof(IsUpdateHostEnabled), value);

    [ObservableProperty]
    private static bool isSearchEnabled = Preferences.Default.Get(nameof(IsSearchEnabled), true);
    async partial void OnIsSearchEnabledChanged(bool value)
    {
        Preferences.Default.Set(nameof(IsSearchEnabled), value);

        await Toast.Make(GlobalConst._SettingsRestartToApplyToastMsg).Show();
    }

    [ObservableProperty]
    private static bool isFirstRunning = Preferences.Default.Get(nameof(IsFirstRunning), true);
    partial void OnIsFirstRunningChanged(bool value) => Preferences.Default.Set(nameof(IsFirstRunning), value);

    [ObservableProperty]
    private static bool isFlagCopied = Preferences.Default.Get(nameof(IsFlagCopied), false);
    partial void OnIsFlagCopiedChanged(bool value) => Preferences.Default.Set(nameof(IsFlagCopied), value);

    [ObservableProperty]
    private static bool isCommandLineExist = Preferences.Default.Get(nameof(IsCommandLineExist), false);
    partial void OnIsCommandLineExistChanged(bool value) => Preferences.Default.Set(nameof(IsCommandLineExist), value);
}