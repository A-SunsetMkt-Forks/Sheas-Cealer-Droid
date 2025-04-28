using Android.Runtime;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Sheas_Cealer_Droid.Consts;
using Sheas_Cealer_Droid.Preses;
using System;

namespace Sheas_Cealer_Droid;

public partial class App : Application
{
    private readonly AppPres AppPres;

    public App()
    {
        InitializeComponent();

        AppPres = new();

        AndroidEnvironment.UnhandledExceptionRaiser += AppAndroidEnvironment_UnhandledExceptionRaiser;

        Resources.Add((ResourceDictionary)Activator.CreateInstance(GlobalConst.ThemeColorDictionary[AppPres.ThemeColorName])!);
    }
    protected override Window CreateWindow(IActivationState? activationState) => new(new AppShell());

    private async void AppAndroidEnvironment_UnhandledExceptionRaiser(object? sender, RaiseThrowableEventArgs e)
    {
        await Toast.Make(string.Format(AppConst._ErrorToastMsg, e.Exception.Message), ToastDuration.Long).Show();

        e.Handled = true;
    }
}