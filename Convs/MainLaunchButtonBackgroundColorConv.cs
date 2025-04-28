using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using System;
using System.Diagnostics;
using System.Globalization;

namespace Sheas_Cealer_Droid.Convs;

internal class MainLaunchButtonBackgroundColorConv : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool? isCommandLineUtd = value as bool?;

        return Application.Current!.RequestedTheme switch
        {
            AppTheme.Light => isCommandLineUtd.HasValue ?
            isCommandLineUtd.Value ? Application.Current.Resources["Primary400"] : Application.Current.Resources["Tertiary400"] : Application.Current.Resources["Gray400"],

            AppTheme.Dark => isCommandLineUtd.HasValue ?
            isCommandLineUtd.Value ? Application.Current.Resources["Primary700"] : Application.Current.Resources["Tertiary700"] : Application.Current.Resources["Gray700"],

            _ => new UnreachableException()
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
}