using Sheas_Cealer_Droid.Consts;
using Sheas_Cealer_Droid.Preses;
using System.IO;
using System.Threading.Tasks;

namespace Sheas_Cealer_Droid.Utils;

internal static class StatusManager
{
    internal static async Task RefreshCurrentStatus(MainPres mainPres, bool cealHostRulesDictContainNull = false)
    {
        if (string.IsNullOrWhiteSpace(await File.ReadAllTextAsync(GlobalConst.CommandLinePath)))
        {
            mainPres.IsCommandLineUtd = null;
            mainPres.StatusMessage = MainConst._InactiveStatusMessage;
        }
        else
        {
            mainPres.IsCommandLineUtd = true;
            mainPres.StatusMessage = cealHostRulesDictContainNull ? MainConst._PartialStatusMessage : MainConst._ActiveStatusMessage;
        }
    }
}