using Microsoft.Maui.Storage;
using System.IO;
using System.Text.RegularExpressions;

namespace Sheas_Cealer_Droid.Consts;

internal abstract partial class MainConst : MainMultilangConst
{
    internal static string AppDataPath => FileSystem.AppDataDirectory;
    internal static string CealHostPath => Path.Combine(AppDataPath, "Cealing-Host-*.json");
    internal static string UpstreamHostPath => Path.Combine(AppDataPath, "Cealing-Host-U.json");
    internal static string LocalHostPath => Path.Combine(AppDataPath, "Cealing-Host-L.json");
    internal static string GithubRepoUrl => "https://github.com/SpaceTimee/Sheas-Cealer-Droid";
    internal static string GithubReleaseUrl => "https://github.com/SpaceTimee/Sheas-Cealer-Droid/releases/latest";
    internal static string GithubMirrorUrl => "https://ghfast.top/";
    internal static string UpdateApiUrl => "https://api.github.com/repos/SpaceTimee/Sheas-Cealer-Droid/releases/latest";
    internal static string UpdateApiUserAgent => "Sheas-Cealer-Droid";
    internal static string DisableCealArg => "--disable-cealing";
    internal static string[] KaomojiShakeArray => ["(＞ ﹏ ＜ *  )", "( ＞ ﹏ ＜ * )", "(* ＞ ﹏ ＜ *)", "( * ＞ ﹏ ＜ )", "(  * ＞ ﹏ ＜)"];

    [GeneratedRegex("^Cealing-Host-")]
    internal static partial Regex CealHostPrefixRegex();
}