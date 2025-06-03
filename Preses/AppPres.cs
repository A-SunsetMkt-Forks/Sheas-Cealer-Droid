using CommunityToolkit.Mvvm.ComponentModel;
using Sheas_Cealer_Droid.Consts;

namespace Sheas_Cealer_Droid.Preses;

internal partial class AppPres : GlobalPres
{
    [ObservableProperty]
    private string downloadSpeed = $"↓ -- {AppConst.TrafficSpeedUnitsArray[0]}";

    [ObservableProperty]
    private string uploadSpeed = $"↑ -- {AppConst.TrafficSpeedUnitsArray[0]}";
}