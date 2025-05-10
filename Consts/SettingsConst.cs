namespace Sheas_Cealer_Droid.Consts;

internal abstract class SettingsConst : SettingsMultilangConst
{
    internal static string[] ThemeColorNameArray => [.. GlobalConst.ThemeColorDictionary.Keys];
    internal static string[] ThemeStateNameArray => [.. GlobalConst.ThemeStateDictionary.Keys];
    internal static string[] LangOptionNameArray => [.. GlobalConst.LangOptionDictionary.Keys];
}