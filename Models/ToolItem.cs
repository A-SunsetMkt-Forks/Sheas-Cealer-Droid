using Microsoft.Maui.Controls;

namespace Sheas_Cealer_Droid.Models;

internal class ToolItem
{
    public string Title { get; init; } = string.Empty;
    public ImageButton? Button { get; init; }
    public string? Content { get; init; } = null;
}