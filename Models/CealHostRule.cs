namespace Sheas_Cealer_Droid.Models;

internal class CealHostRule
{
    public string Name { get; internal set; } = string.Empty;
    public string Domain { get; internal set; } = string.Empty;
    public string? Sni { get; internal set; } = string.Empty;
    public string Ip { get; internal set; } = string.Empty;
}