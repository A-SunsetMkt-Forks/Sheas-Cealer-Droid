namespace Sheas_Cealer_Droid.Consts;

internal abstract class AdbConst : AdbMultilangConst
{
    internal static string AdbCommand => @$"adb shell ""touch {MainConst.CommandLinePath} && chmod 666 {MainConst.CommandLinePath}""";
}