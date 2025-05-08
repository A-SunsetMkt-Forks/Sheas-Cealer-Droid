using Microsoft.Maui.Controls;
using Sheas_Cealer_Droid.Consts;
using System;
using System.Globalization;

namespace Sheas_Cealer_Droid.Convs;

internal class AppAdbShellContentTitleConv : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool isFirstRunning = (bool)value!;

        return isFirstRunning ? AppConst.AdbShellContentTitle : AppConst.BrowserShellContentTitle;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
}