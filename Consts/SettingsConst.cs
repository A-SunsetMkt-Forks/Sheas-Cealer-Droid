namespace Sheas_Cealer_Droid.Consts;

internal abstract class SettingsConst : SettingsMultilangConst
{
    internal static string[] ThemeColorNameArray => [.. GlobalConst.ThemeColorDictionary.Keys];
}