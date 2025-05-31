using Sheas_Cealer_Droid.Consts;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Sheas_Cealer_Droid.Utils;

internal static class CommandLineWriter
{
    private static async Task Write(string commandLine) => await File.WriteAllTextAsync(GlobalConst.CommandLinePath, commandLine).ConfigureAwait(false);

    internal static async Task Write(string browserName, string cealArgs, string extraArgs) =>
        await Write($"{browserName.ToLowerInvariant()} " +
            (extraArgs.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Contains(MainConst.DisableCealArg) ? extraArgs : $"{cealArgs} {extraArgs}")).ConfigureAwait(false);

    internal static async Task Clear() => await Write(string.Empty).ConfigureAwait(false);
}