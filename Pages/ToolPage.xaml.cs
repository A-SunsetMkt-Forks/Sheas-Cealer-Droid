using CommunityToolkit.Maui.Alerts;
using Microsoft.Maui.Controls;
using Sheas_Cealer_Droid.Consts;
using System;
using System.Net.NetworkInformation;
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
}