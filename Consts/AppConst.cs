namespace Sheas_Cealer_Droid.Consts;

internal abstract class AppConst : AppMultilangConst
{
    internal static string[] TrafficSpeedUnitsArray => ["B/s", "KB/s", "MB/s", "GB/s", "TB/s", "PB/s", "EB/s"];
    internal static string DefaultOffDownloadSpeed => $"↓ -- {TrafficSpeedUnitsArray[0]}";
    internal static string DefaultOffUploadSpeed => $"↑ -- {TrafficSpeedUnitsArray[0]}";
    internal static string DefaultOnDownloadSpeed => $"↓ 0 {TrafficSpeedUnitsArray[0]}";
    internal static string DefaultOnUploadSpeed => $"↑ 0 {TrafficSpeedUnitsArray[0]}";
}