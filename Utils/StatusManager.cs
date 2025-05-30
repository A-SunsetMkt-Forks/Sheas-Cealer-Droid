using Sheas_Cealer_Droid.Consts;
using System.IO;
using System.Threading.Tasks;

namespace Sheas_Cealer_Droid.Utils;

internal static class StatusManager
{
    internal static async Task<(bool? isCommandLineUtd, string statusMessage)> RefreshCurrentStatus(bool cealHostRulesDictContainNull = false) =>
        string.IsNullOrWhiteSpace(await File.ReadAllTextAsync(GlobalConst.CommandLinePath).ConfigureAwait(false)) ?
                (null, MainConst._InactiveStatusMessage) : (true, cealHostRulesDictContainNull ? MainConst._PartialStatusMessage : MainConst._ActiveStatusMessage);
}