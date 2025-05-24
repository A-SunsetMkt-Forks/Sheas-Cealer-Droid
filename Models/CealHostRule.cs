namespace Sheas_Cealer_Droid.Models;

internal class CealHostRule
{
    internal CealHostRule(string? name, string domains, string? sni, string ip)
    {
        Name = name;
        Domains = domains;
        Sni = sni;
        Ip = ip;
    }

    internal CealHostRule(string domains, string? sni, string ip) : this(null, domains, sni, ip) { }

    public string? Name { get; internal set; }
    public string Domains { get; internal set; }
    public string? Sni { get; internal set; }
    public string Ip { get; internal set; }
}