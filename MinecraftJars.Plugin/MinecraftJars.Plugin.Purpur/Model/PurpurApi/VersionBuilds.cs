using System.Text.Json.Serialization;

namespace MinecraftJars.Plugin.Purpur.Model.PurpurApi;

internal class VersionBuilds
{
    [JsonPropertyName("builds")]
    public Builds Builds { get; set; } = new();

    [JsonPropertyName("project")]
    public string? Project { get; set; }

    [JsonPropertyName("version")]
    public string? Version { get; set; }    
}