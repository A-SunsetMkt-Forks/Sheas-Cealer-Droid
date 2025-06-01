using System.Net;
using System.Net.Sockets;

namespace Sheas_Cealer_Droid.Utils;

internal static class ReservedIpChecker
{
    internal static bool IsReversedIp(IPAddress ip)
    {
        byte[] ipByteArray = ip.GetAddressBytes();

        return ip.AddressFamily == AddressFamily.InterNetwork && (
            IPAddress.IsLoopback(ip) ||
            ipByteArray[0] == 0 ||
            ipByteArray[0] == 10 ||
            ipByteArray[0] == 100 && ipByteArray[1] >= 64 && ipByteArray[1] <= 127 ||
            ipByteArray[0] == 127 ||
            ipByteArray[0] == 169 && ipByteArray[1] == 254 ||
            ipByteArray[0] == 172 && ipByteArray[1] >= 16 && ipByteArray[1] <= 31 ||
            ipByteArray[0] == 192 && ipByteArray[1] == 0 && ipByteArray[2] == 0 ||
            ipByteArray[0] == 192 && ipByteArray[1] == 0 && ipByteArray[2] == 2 ||
            ipByteArray[0] == 192 && ipByteArray[1] == 88 && ipByteArray[2] == 99 ||
            ipByteArray[0] == 192 && ipByteArray[1] == 168 ||
            ipByteArray[0] == 198 && ipByteArray[1] >= 18 && ipByteArray[1] <= 19 ||
            ipByteArray[0] == 198 && ipByteArray[1] == 51 && ipByteArray[2] == 100 ||
            ipByteArray[0] == 203 && ipByteArray[1] == 0 && ipByteArray[2] == 113 ||
            ipByteArray[0] >= 224 && ipByteArray[0] <= 239 ||
            ipByteArray[0] == 233 && ipByteArray[1] == 252 && ipByteArray[2] == 0 ||
            ipByteArray[0] >= 240
        ) ||

        ip.AddressFamily == AddressFamily.InterNetworkV6 && (
            IPAddress.IsLoopback(ip) ||
            ip.Equals(IPAddress.IPv6None) ||
            ip.IsIPv4MappedToIPv6 ||
            ip.IsIPv6LinkLocal ||
            ip.IsIPv6Multicast ||
            ip.IsIPv6SiteLocal ||
            ip.IsIPv6Teredo ||
            ip.IsIPv6UniqueLocal
        );
    }
}