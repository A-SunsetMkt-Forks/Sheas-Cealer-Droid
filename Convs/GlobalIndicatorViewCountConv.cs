using Microsoft.Maui.Controls;
using System;
using System.Globalization;

namespace Sheas_Cealer_Droid.Convs;

internal class GlobalIndicatorViewCountConv : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool isFirstRunning = (bool)value!;

        return isFirstRunning ? 4 : 2;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
}