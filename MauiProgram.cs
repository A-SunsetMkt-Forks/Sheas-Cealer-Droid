using CommunityToolkit.Maui;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;

namespace Sheas_Cealer_Droid;

internal static class MauiProgram
{
    internal static MauiApp CreateMauiApp() => MauiApp.CreateBuilder().UseMauiApp<App>().UseMauiCommunityToolkit().Build();
}