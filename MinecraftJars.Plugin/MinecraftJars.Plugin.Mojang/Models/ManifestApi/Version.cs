using System.Text.Json.Serialization;

namespace MinecraftJars.Plugin.Mojang.Models.ManifestApi;

#pragma warning disable CS8618
internal class Version
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("time")]
    public DateTime Time { get; set; }

    [JsonPropertyName("releaseTime")]
    public DateTime ReleaseTime { get; set; }

    [JsonPropertyName("sha1")]
    public string? Sha1 { get; set; }

    [JsonPropertyName("complianceLevel")]
    public int? ComplianceLevel { get; set; }    
}
#pragma warning restore CS8618