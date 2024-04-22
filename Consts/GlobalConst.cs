using System.Collections.ObjectModel;

namespace Sheas_Cealer_Droid.Consts;

internal abstract class GlobalConst : GlobalMultilangConst
{
    internal static ObservableCollection<string> DefaultBrowserNameCollection => ["Chrome", "Edge", "Brave", "Opera", "Yandex", "Vivaldi", "Kiwi", "Whale", "Bromite", "Twinkstar", "Lemur", _BrowserNameCollectionCustomTitle];
    internal static string[] SkipWarningArray => [_SkipWarning1ToastMsg, _SkipWarning2ToastMsg, _SkipWarning3ToastMsg, _SkipWarning4ToastMsg];
}