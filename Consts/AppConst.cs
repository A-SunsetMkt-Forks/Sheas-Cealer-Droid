using Sheas_Cealer_Droid.Pages;
using System;

namespace Sheas_Cealer_Droid.Consts;

internal abstract class AppConst : AppMultilangConst
{
    internal static Type[] DetailPageTypeArray => [typeof(SettingsPage), typeof(AboutPage)];
}