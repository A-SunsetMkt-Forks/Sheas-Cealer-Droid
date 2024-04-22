using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Sheas_Cealer_Droid.Consts;
using System;
using System.Linq;

namespace Sheas_Cealer_Droid;

public partial class AppShell : Shell
{
    internal AppShell()
    {
        InitializeComponent();

        CurrentItem = Preferences.Default.Get("IsFirstRunning", true) ? GuideTabBar : MainShellContent;
    }
    private void AppShell_Loaded(object sender, EventArgs e)
    {
        foreach (Type detailPageType in AppConst.DetailPageTypeArray)
            Routing.RegisterRoute(detailPageType.Name, detailPageType);
    }

    private void AppShell_Navigating(object sender, ShellNavigatingEventArgs e)
    {
        string targetPageName = e.Target.Location.OriginalString.TrimStart('/');

        if (e.Source == ShellNavigationSource.ShellItemChanged && AppConst.DetailPageTypeArray.Any(t => t.Name == targetPageName))
        {
            GoToAsync(targetPageName);

            e.Cancel();
        }
    }
}