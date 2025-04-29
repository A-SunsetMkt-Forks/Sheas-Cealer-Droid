using Microsoft.Maui.Controls;
using Sheas_Cealer_Droid.Consts;
using System;
using System.Globalization;

namespace Sheas_Cealer_Droid.Convs;

internal class FlagGuideLabelTextConv : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        string browserName = (string)value!;

        return string.Format(FlagConst.GuideLabelText, browserName);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
}