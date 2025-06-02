using Microsoft.Maui.Controls;
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

    internal AppShell(AppPres appPres)
    {
        InitializeComponent();

        BindingContext = AppPres = appPres;

        CurrentItem = AppPres.IsFirstRunning ? BrowserShellContent :
            File.Exists(GlobalConst.CommandLinePath) ? MainShellContent : AdbShellContent;
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
}