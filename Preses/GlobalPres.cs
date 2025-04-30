using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Sheas_Cealer_Droid.Consts;
using Sheas_Cealer_Droid.Models;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;

namespace Sheas_Cealer_Droid.Preses;

internal abstract partial class GlobalPres : ObservableObject
{
    protected GlobalPres() => BrowserNameCollection.CollectionChanged += BrowserNameCollection_CollectionChanged;

    [ObservableProperty]
    private static string themeColorName = Preferences.Default.Get(nameof(ThemeColorName), GlobalConst._ThemeColorRedName);
    async partial void OnThemeColorNameChanged(string value)
    {
        Preferences.Default.Set(nameof(ThemeColorName), value);

        await Toast.Make(GlobalConst._ThemeColorRestartToApplyToastMsg).Show();
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

    private static readonly ObservableCollection<CealHostRule> cealHostRulesCollection = [];
    [SuppressMessage("Performance", "CA1822"), SuppressMessage("CodeQuality", "IDE0079")]
    public ObservableCollection<CealHostRule> CealHostRulesCollection => cealHostRulesCollection;

    [ObservableProperty]
    private static bool isFlagCopied = false;

    [ObservableProperty]
    private static bool isCommandLineExist = false;
}