using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;

namespace Sheas_Cealer_Droid;

public partial class AppShell : Shell
{
    private readonly List<Type> DetailPageTypeArray = [];

    internal AppShell()
    {
        InitializeComponent();

        CurrentItem = Preferences.Default.Get("IsFirstRunning", true) ? GuideTabBar : MainShellContent;
    }

    private async void DetailMenuItem_Clicked(object sender, EventArgs e)
    {
        MenuItem senderMenuItem = (MenuItem)sender;
        Type targetDetailPageType = (Type)senderMenuItem.CommandParameter;

        if (!DetailPageTypeArray.Contains(targetDetailPageType))
        {
            Routing.RegisterRoute(targetDetailPageType.Name, targetDetailPageType);

            DetailPageTypeArray.Add(targetDetailPageType);
        }

        await GoToAsync(targetDetailPageType.Name);
    }
}