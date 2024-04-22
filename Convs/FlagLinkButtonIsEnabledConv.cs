using Microsoft.Maui.Controls;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Sheas_Cealer_Droid.Convs;

internal class FlagLinkButtonIsEnabledConv : IValueConverter
{
    [SuppressMessage("Style", "IDE0019")]
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        string? browserName = value as string;

        return browserName != null;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
}