using System.Text.Json.Serialization;

namespace MinecraftJars.Plugin.Mojang.Models.MojangApi;

internal class Latest
{
    [JsonPropertyName("release")]
    public string? Release { get; set; }

    [JsonPropertyName("snapshot")]
    public string? Snapshot { get; set; }    
}