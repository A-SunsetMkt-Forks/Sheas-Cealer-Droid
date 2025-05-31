using CommunityToolkit.Maui.Alerts;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Controls;
using Ona_Core;
using Sheas_Cealer_Droid.Consts;
using Sheas_Cealer_Droid.Utils;
using System;
using System.Net;
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
        string? dohDomainName = await Shell.Current.CurrentPage.DisplayPromptAsync(ToolConst._DohDomainNamePopupTitle, ToolConst._DohDomainNamePopupMsg, GlobalConst._PopupAcceptText, GlobalConst._PopupCancelText);

        if (string.IsNullOrWhiteSpace(dohDomainName))
            return;

        if (!dohDomainName.StartsWith("https://") && !dohDomainName.StartsWith("http://"))
            dohDomainName = "https://" + dohDomainName;

        try { dohDomainName = new Uri(dohDomainName).DnsSafeHost; }
        catch
        {
            await Toast.Make(ToolConst._DohResolveInvalidToastMsg).Show();

            return;
        }

        if (JsonDocument.Parse(await Http.GetAsync<string>($"{ToolConst.DohUrl}{dohDomainName}", App.AppClient)).RootElement.TryGetProperty("Answer", out JsonElement dohAnswers))
        {
            await Clipboard.Default.SetTextAsync(dohAnswers[0].GetProperty("data").GetString());
            await Toast.Make(ToolConst._DohResolveSuccessToastMsg).Show();
        }
        else
            await Toast.Make(ToolConst._DohResolveErrorToastMsg).Show();
    }

    internal static ICommand PingImageButton_ClickedCommand => new Command(async () => await PingImageButton_Clicked(null!, null!));
    private static async Task PingImageButton_Clicked(object sender, EventArgs e)
    {
        string? pingHostName = await Shell.Current.CurrentPage.DisplayPromptAsync(ToolConst._PingHostNamePopupTitle, ToolConst._PingHostNamePopupMsg, GlobalConst._PopupAcceptText, GlobalConst._PopupCancelText);

        if (string.IsNullOrWhiteSpace(pingHostName))
            return;

        if (!pingHostName.StartsWith("https://") && !pingHostName.StartsWith("http://"))
            pingHostName = "https://" + pingHostName;

        try { pingHostName = new Uri(pingHostName).DnsSafeHost; }
        catch
        {
            await Toast.Make(ToolConst._PingTestInvalidToastMsg).Show();

            return;
        }

        if (!IPAddress.TryParse(pingHostName, out IPAddress? pingTestIp))
        {
            try { pingTestIp = (await Dns.GetHostAddressesAsync(pingHostName))[0]; }
            catch
            {
                await Toast.Make(ToolConst._PingTestErrorToastMsg).Show();

                return;
            }

            if (ReservedIpChecker.IsReversedIp(pingTestIp))
            {
                await Toast.Make(ToolConst._PingTestReversedToastMsg).Show();

                return;
            }
        }

        IPStatus pingTestStatus;

        try { pingTestStatus = (await new Ping().SendPingAsync(pingTestIp)).Status; }
        catch { pingTestStatus = IPStatus.Unknown; }

        if (pingTestStatus == IPStatus.Success)
            await Toast.Make(ToolConst._PingTestSuccessToastMsg).Show();
        else
            await Toast.Make(ToolConst._PingTestErrorToastMsg).Show();
    }

    internal static ICommand ReserveImageButton_ClickedCommand => new Command(async () => await ReserveImageButton_Clicked(null!, null!));
    private static async Task ReserveImageButton_Clicked(object sender, EventArgs e)
    {
        string? reserveCheckHost = await Shell.Current.CurrentPage.DisplayPromptAsync(ToolConst._ReserveCheckHostPopupTitle, ToolConst._ReserveCheckHostPopupMsg, GlobalConst._PopupAcceptText, GlobalConst._PopupCancelText);

        if (string.IsNullOrWhiteSpace(reserveCheckHost))
            return;

        if (!IPAddress.TryParse(reserveCheckHost, out IPAddress? reserveCheckIp))
            await Toast.Make(ToolConst._ReserveCheckInvalidToastMsg).Show();
        else if (ReservedIpChecker.IsReversedIp(reserveCheckIp))
            await Toast.Make(ToolConst._ReserveCheckReversedToastMsg).Show();
        else
            await Toast.Make(ToolConst._ReserveCheckNonreversedToastMsg).Show();
    }
}