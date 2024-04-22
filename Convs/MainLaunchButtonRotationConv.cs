using Microsoft.Maui.Controls;
using System;
using System.Globalization;

namespace Sheas_Cealer_Droid.Convs;

internal class MainLaunchButtonRotationConv : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool? isCommandLineUtd = value as bool?;

        return isCommandLineUtd.HasValue ? isCommandLineUtd.Value ? 0 : 90 : 180;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
}