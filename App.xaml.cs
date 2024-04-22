using Android.Runtime;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Sheas_Cealer_Droid.Consts;

namespace Sheas_Cealer_Droid;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        AndroidEnvironment.UnhandledExceptionRaiser += AppAndroidEnvironment_UnhandledExceptionRaiser;
    }
    protected override Window CreateWindow(IActivationState? activationState) => new(new AppShell());

    private async void AppAndroidEnvironment_UnhandledExceptionRaiser(object? sender, RaiseThrowableEventArgs e)
    {
        await Toast.Make(string.Format(AppConst._ErrorToastMsg, e.Exception.Message), ToastDuration.Long).Show();

        e.Handled = true;
    }
}