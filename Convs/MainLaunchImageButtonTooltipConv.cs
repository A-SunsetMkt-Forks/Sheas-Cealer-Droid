using Microsoft.Maui.Controls;
using Sheas_Cealer_Droid.Consts;
using System;
using System.Globalization;

namespace Sheas_Cealer_Droid.Convs;

internal class MainLaunchImageButtonTooltipConv : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool? isCommandLineUtd = value as bool?;

        return isCommandLineUtd.HasValue ?
            isCommandLineUtd.Value ? MainConst.LaunchImageButtonActiveTooltip : MainConst.LaunchImageButtonUpdateTooltip : MainConst.LaunchImageButtonInactiveTooltip;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
}