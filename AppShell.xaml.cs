using Android.Net;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching;
using Sheas_Cealer_Droid.Consts;
using Sheas_Cealer_Droid.Preses;
using System;
using System.Collections.Generic;
using System.IO;

namespace Sheas_Cealer_Droid;

internal partial class AppShell : Shell
{
    private readonly AppPres AppPres;
    private readonly List<Type> DetailPageTypeArray = [];
    private readonly IDispatcherTimer TrafficSpeedTimer = Application.Current!.Dispatcher.CreateTimer();
    private long LastRxBytes = TrafficStats.TotalRxBytes;
    private long LastTxBytes = TrafficStats.TotalTxBytes;

    internal AppShell(AppPres appPres)
    {
        InitializeComponent();

        BindingContext = AppPres = appPres;

        CurrentItem = AppPres.IsFirstRunning ? BrowserShellContent :
            File.Exists(GlobalConst.CommandLinePath) ? MainShellContent : AdbShellContent;
    }
    private void AppShell_Loaded(object sender, EventArgs e)
    {
        TrafficSpeedTimer.Interval = TimeSpan.FromSeconds(1);
        TrafficSpeedTimer.Tick += TrafficSpeedTimer_Tick;
        TrafficSpeedTimer.Start();
    }

    private async void DetailMenuItem_Clicked(object sender, EventArgs e)
    {
        MenuItem senderMenuItem = (MenuItem)sender;
        Type targetDetailPageType = (Type)senderMenuItem.CommandParameter!;

        if (!DetailPageTypeArray.Contains(targetDetailPageType))
        {
            Routing.RegisterRoute(targetDetailPageType.Name, targetDetailPageType);

            DetailPageTypeArray.Add(targetDetailPageType);
        }

        await GoToAsync(targetDetailPageType.Name);
    }

    private void HeaderTapGestureRecognizer_Tapped(object sender, TappedEventArgs e) => FlyoutIsPresented = false;

    private void TrafficSpeedTimer_Tick(object? sender, EventArgs e)
    {
        long downloadSpeed = TrafficStats.TotalRxBytes - LastRxBytes;
        long uploadSpeed = TrafficStats.TotalTxBytes - LastTxBytes;

        int downloadSpeedUnitsArrayIndex = downloadSpeed >= 1024 ? (int)Math.Log(downloadSpeed, 1024) : 0;
        int uploadSpeedUnitsArrayIndex = uploadSpeed >= 1024 ? (int)Math.Log(uploadSpeed, 1024) : 0;

        AppPres.DownloadSpeed = $"↓ {Math.Ceiling(downloadSpeed / Math.Pow(1024, downloadSpeedUnitsArrayIndex) * 10) / 10} {AppConst.TrafficSpeedUnitsArray[downloadSpeedUnitsArrayIndex]}";
        AppPres.UploadSpeed = $"↑ {Math.Ceiling(uploadSpeed / Math.Pow(1024, uploadSpeedUnitsArrayIndex) * 10) / 10} {AppConst.TrafficSpeedUnitsArray[uploadSpeedUnitsArrayIndex]}";

        LastRxBytes = TrafficStats.TotalRxBytes;
        LastTxBytes = TrafficStats.TotalTxBytes;
    }
}