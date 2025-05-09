using CommunityToolkit.Maui.Alerts;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Controls;
using Sheas_Cealer_Droid.Consts;
using Sheas_Cealer_Droid.Models;

namespace Sheas_Cealer_Droid.Pages;

public partial class AboutPage : ContentPage
{
    public AboutPage() => InitializeComponent();

    private async void CopyImageButton_Clicked(object sender, System.EventArgs e)
    {
        ImageButton senderImageButton = (ImageButton)sender;

        await Clipboard.Default.SetTextAsync(((AboutInfo)senderImageButton.BindingContext).Url);
        await Toast.Make(GlobalConst._LinkCopiedToastMsg).Show();
    }
    private async void GotoImageButton_Clicked(object sender, System.EventArgs e)
    {
        ImageButton senderImageButton = (ImageButton)sender;

        await Browser.Default.OpenAsync(((AboutInfo)senderImageButton.BindingContext).Url!);
    }
}