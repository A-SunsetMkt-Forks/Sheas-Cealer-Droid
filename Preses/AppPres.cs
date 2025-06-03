using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Storage;
using Sheas_Cealer_Droid.Consts;

namespace Sheas_Cealer_Droid.Preses;

internal partial class AppPres : GlobalPres
{
    [ObservableProperty]
    private string downloadSpeed = Preferences.Default.Get("IsTrafficSpeedTimerRunning", true) ? AppConst.DefaultOnDownloadSpeed : AppConst.DefaultOffDownloadSpeed;

    [ObservableProperty]
    private string uploadSpeed = Preferences.Default.Get("IsTrafficSpeedTimerRunning", true) ? AppConst.DefaultOnUploadSpeed : AppConst.DefaultOffUploadSpeed;
}