using System.Text.Json.Serialization;

namespace MinecraftJars.Plugin.Fabric.Model.FabricApi;

public class Loader
{
    [JsonPropertyName("separator")]
    public string? Separator { get; set; }

    [JsonPropertyName("build")]
    public int Build { get; set; }

    [JsonPropertyName("maven")]
    public string? Maven { get; set; }

    [JsonPropertyName("version")] 
    public string Version { get; set; } = string.Empty;

    [JsonPropertyName("stable")]
    public bool Stable { get; set; }
}