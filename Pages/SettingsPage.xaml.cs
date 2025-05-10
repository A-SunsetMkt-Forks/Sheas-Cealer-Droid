using CommunityToolkit.Maui.Alerts;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Controls;
using Sheas_Cealer_Droid.Consts;
using Sheas_Cealer_Droid.Preses;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sheas_Cealer_Droid.Pages;

public partial class SettingsPage : ContentPage
{
    private readonly SettingsPres SettingsPres;

    public SettingsPage()
    {
        InitializeComponent();

        BindingContext = SettingsPres = new();
    }

    internal static ICommand UpstreamUrlEntry_CompletedCommand => new Command(sender => UpstreamUrlEntry_Completed(sender, null!));
    private static void UpstreamUrlEntry_Completed(object sender, EventArgs e)
    {
        Entry senderEntry = (Entry)sender;

        senderEntry.Unfocus();
    }

    internal static ICommand LinkImageButton_ClickedCommand => new Command(async () => await LinkImageButton_Clicked(null!, null!));
    private static async Task LinkImageButton_Clicked(object sender, EventArgs e)
    {
        await Clipboard.Default.SetTextAsync(GlobalConst.FlagUrl);
        await Toast.Make(GlobalConst._LinkCopiedToastMsg).Show();
    }

    internal static ICommand CommandImageButton_ClickedCommand => new Command(async () => await CommandImageButton_Clicked(null!, null!));
    private static async Task CommandImageButton_Clicked(object sender, EventArgs e)
    {
        await Clipboard.Default.SetTextAsync(GlobalConst.AdbCommand);
        await Toast.Make(GlobalConst._CommandCopiedToastMsg).Show();
    }
}