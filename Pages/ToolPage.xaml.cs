using CommunityToolkit.Maui.Alerts;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Controls;
using Ona_Core;
using Sheas_Cealer_Droid.Consts;
using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sheas_Cealer_Droid.Pages;

public partial class ToolPage : ContentPage
{
    public ToolPage() => InitializeComponent();

    internal static ICommand Ipv6ImageButton_ClickedCommand => new Command(async () => await Ipv6ImageButton_Clicked(null!, null!));
    private static async Task Ipv6ImageButton_Clicked(object sender, EventArgs e)
    {
        IPStatus ipv6TestStatus;

        try { ipv6TestStatus = (await new Ping().SendPingAsync(ToolConst.Ipv6TestIp)).Status; }
        catch { ipv6TestStatus = IPStatus.Unknown; }

        if (ipv6TestStatus == IPStatus.Success)
            await Toast.Make(ToolConst._Ipv6TestSuccessToastMsg).Show();
        else
            await Toast.Make(ToolConst._Ipv6TestErrorToastMsg).Show();
    }

    internal static ICommand DohImageButton_ClickedCommand => new Command(async () => await DohImageButton_Clicked(null!, null!));
    private static async Task DohImageButton_Clicked(object sender, EventArgs e)
    {
        string? dohDomainName = (await Shell.Current.CurrentPage.DisplayPromptAsync(ToolConst._DohDomainNamePopupTitle, ToolConst._DohDomainNamePopupMsg, GlobalConst._PopupAcceptText, GlobalConst._PopupCancelText))?.
            ToLowerInvariant().Trim().TrimStart("http://".ToCharArray()).TrimStart("https://".ToCharArray());

        int portStartIndex = dohDomainName?.IndexOf(':') ?? -1;

        if (portStartIndex != -1)
            dohDomainName = dohDomainName![..portStartIndex];

        int pathStartIndex = dohDomainName?.IndexOf('/') ?? -1;

        if (pathStartIndex != -1)
            dohDomainName = dohDomainName![..pathStartIndex];

        if (string.IsNullOrWhiteSpace(dohDomainName))
            return;

        dohDomainName = new([.. dohDomainName.Where(c => !char.IsWhiteSpace(c))]);

        if (JsonDocument.Parse(await Http.GetAsync<string>($"{ToolConst.DohUrl}{dohDomainName}", App.AppClient)).RootElement.TryGetProperty("Answer", out JsonElement dohAnswers))
        {
            await Clipboard.Default.SetTextAsync(dohAnswers[0].GetProperty("data").GetString());
            await Toast.Make(ToolConst._DohResolveSuccessToastMsg).Show();
        }
        else
            await Toast.Make(ToolConst._DohResolveErrorToastMsg).Show();
    }
}