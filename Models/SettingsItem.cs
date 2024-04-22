using Microsoft.Maui.Controls;

namespace Sheas_Cealer_Droid.Models;

internal class SettingsItem
{
    public string Title { get; init; } = string.Empty;
    public View? View { get; init; }
    public string? Content { get; init; } = null;
}