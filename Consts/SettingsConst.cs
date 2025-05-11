namespace Sheas_Cealer_Droid.Consts;

internal abstract class SettingsConst : SettingsMultilangConst
{
    internal static string[] ThemeColorNameArray => [.. GlobalConst.ThemeColorDictionary.Keys];
    internal static string[] ThemeStateNameArray => [.. GlobalConst.ThemeStateDictionary.Keys];
    internal static string[] LangOptionNameArray => [.. GlobalConst.LangOptionDictionary.Keys];

    internal static string UpstreamUrlRegexPattern => "^(https?:\\/\\/)?[a-zA-Z0-9](-*[a-zA-Z0-9])*(\\.[a-zA-Z0-9](-*[a-zA-Z0-9])*)*(:\\d{1,5})?(\\/[a-zA-Z0-9.\\-_\\~\\!\\$\\&\\'\\(\\)\\*\\+\\,\\;\\=\\:\\@\\%]*)*$";
}