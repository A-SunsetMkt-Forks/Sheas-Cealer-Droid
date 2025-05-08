using Microsoft.Maui.Controls;
using Sheas_Cealer_Droid.Consts;
using System;
using System.Globalization;

namespace Sheas_Cealer_Droid.Convs;

internal class ReadyGuideLabelTextConv : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool isCommandLineExist = (bool)value!;

        return isCommandLineExist ? ReadyConst.GuideLabelText : ReadyConst.GuideLabelTextIsCommandLineExistFallback;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
}