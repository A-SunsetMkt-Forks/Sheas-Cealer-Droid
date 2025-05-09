using Microsoft.Maui.Storage;
using System.IO;

namespace Sheas_Cealer_Droid.Consts;

internal abstract class MainConst : MainMultilangConst
{
    internal static string CealHostPath => Path.Combine(FileSystem.AppDataDirectory, "Cealing-Host-*.json");
    internal static string UpstreamHostPath => Path.Combine(FileSystem.AppDataDirectory, "Cealing-Host-U.json");
    internal static string GithubRepoUrl => "https://github.com/SpaceTimee/Sheas-Cealer-Droid";
    internal static string GithubReleaseUrl => "https://github.com/SpaceTimee/Sheas-Cealer-Droid/releases/latest";
    internal static string UpdateApiUrl => "https://api.github.com/repos/SpaceTimee/Sheas-Cealer-Droid/releases/latest";
    internal static string UpdateApiUserAgent => "Sheas-Cealer-Droid";
}