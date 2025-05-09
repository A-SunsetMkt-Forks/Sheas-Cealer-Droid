using Ona_Core;
using Sheas_Cealer_Droid.Consts;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sheas_Cealer_Droid.Utils;

internal static class UpdateChecker
{
    internal static async Task<bool> CheckUpdate(HttpClient httpClient)
    {
        httpClient.DefaultRequestHeaders.Add("User-Agent", GlobalConst.UpdateApiUserAgent);

        JsonElement updateInfoObject = JsonDocument.Parse(await Http.GetAsync<string>(GlobalConst.UpdateApiUrl, httpClient)).RootElement;

        httpClient.DefaultRequestHeaders.Clear();

        foreach (JsonProperty updateInfoContent in updateInfoObject.EnumerateObject())
            if (updateInfoContent.Name == "name" && updateInfoContent.Value.ToString() != AboutConst.VersionAboutInfoContent)
                return true;

        return false;
    }
}