using CommunityToolkit.Mvvm.ComponentModel;
using Sheas_Cealer_Droid.Utils;

namespace Sheas_Cealer_Droid.Preses;

internal partial class MainPres : GlobalPres
{
    internal MainPres() => (IsCommandLineUtd, StatusMessage) = StatusManager.RefreshCurrentStatus().GetAwaiter().GetResult();

    [ObservableProperty]
    private bool isPageLoading = true;

    [ObservableProperty]
    private string statusMessage = string.Empty;

    [ObservableProperty]
    private double statusProgress = 1;

    [ObservableProperty]
    private bool isHostCollectionAtBottom = false;
}