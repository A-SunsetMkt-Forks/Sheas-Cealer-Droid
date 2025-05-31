using Android.Runtime;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Sheas_Cealer_Droid.Consts;
using Sheas_Cealer_Droid.Preses;
using System;
using System.Globalization;
using System.Net.Http;

namespace Sheas_Cealer_Droid;

public partial class App : Application
{
    internal static readonly HttpClient AppClient = new(new HttpClientHandler { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator });

    internal static string CealArgs = string.Empty;

    private readonly AppPres AppPres;

    public App()
    {
        InitializeComponent();

        AppPres = new();

        AndroidEnvironment.UnhandledExceptionRaiser += AppAndroidEnvironment_UnhandledExceptionRaiser;

        if (GlobalConst.ThemeColorDictionary.TryGetValue(AppPres.ThemeColorName, out Type? themeColorType))
            Resources.Add((ResourceDictionary)Activator.CreateInstance(themeColorType)!);

        if (GlobalConst.ThemeStateDictionary.TryGetValue(AppPres.ThemeStateName, out AppTheme themeStateTheme))
            UserAppTheme = themeStateTheme;

        if (GlobalConst.LangOptionDictionary.TryGetValue(AppPres.LangOptionName, out string? langOptionCulture))
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.DefaultThreadCurrentUICulture = langOptionCulture == null ? null : new(langOptionCulture);
    }
    protected override Window CreateWindow(IActivationState? activationState) => new(new AppShell(AppPres));

    private async void AppAndroidEnvironment_UnhandledExceptionRaiser(object? sender, RaiseThrowableEventArgs e)
    {
        await Toast.Make(string.Format(AppConst._ErrorToastMsg, e.Exception.Message), ToastDuration.Long).Show();

        e.Handled = true;
    }
}