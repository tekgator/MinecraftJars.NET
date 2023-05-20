using System.Text.Json.Serialization;

namespace MinecraftJars.Plugin.Mojang.Models.ManifestApi;

internal class Latest
{
    [JsonPropertyName("release")]
    public string? Release { get; set; }

    [JsonPropertyName("snapshot")]
    public string? Snapshot { get; set; }    
}