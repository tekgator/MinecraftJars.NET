using System.Text.Json.Serialization;

namespace MinecraftJars.Plugin.Purpur.Model.PurpurApi;

internal class Builds
{
    [JsonPropertyName("all")]
    public List<string> All { get; set; } = new();

    [JsonPropertyName("latest")]
    public string? Latest { get; set; }    
}