using Android.App;
using Android.Content.PM;
using Android.OS;
using Microsoft.Maui;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Platform;
using System.Diagnostics.CodeAnalysis;

namespace Sheas_Cealer_Droid.Plats.Android;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    [SuppressMessage("Interoperability", "CA1422"), SuppressMessage("CodeQuality", "IDE0079")]
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        Window?.SetNavigationBarColor(((Color)(Microsoft.Maui.Controls.Application.Current!.RequestedTheme == AppTheme.Light ? Microsoft.Maui.Controls.Application.Current.Resources["Primary50"] : Microsoft.Maui.Controls.Application.Current.Resources["Gray850"])).ToPlatform());
    }
}