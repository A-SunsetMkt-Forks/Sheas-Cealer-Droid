using Sheas_Cealer_Droid.Colors;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sheas_Cealer_Droid.Consts;

internal abstract class GlobalConst : GlobalMultilangConst
{
    internal static Dictionary<string, Type> ThemeColorDictionary => new()
    {
        { _ThemeColorRedName, typeof(RedColor) },
        { _ThemeColorYellowName, typeof(YellowColor) },
        { _ThemeColorBlueName, typeof(BlueColor) },
        { _ThemeColorGreenName, typeof(GreenColor) },
        { _ThemeColorOrangeName, typeof(OrangeColor) }
    };
    internal static ObservableCollection<string> DefaultBrowserNameCollection => ["Chrome", "Edge", "Brave", "Opera", "Yandex", "Vivaldi", "Kiwi", "Whale", "Bromite", "Twinkstar", "Lemur", _BrowserNameCollectionCustomTitle];
    internal static string DefaultUpstreamUrl => "https://gitlab.com/SpaceTimee/Cealing-Host/raw/main/Cealing-Host.json";
    internal static string[] SkipWarningArray => [_SkipWarning1ToastMsg, _SkipWarning2ToastMsg, _SkipWarning3ToastMsg, _SkipWarning4ToastMsg];
}