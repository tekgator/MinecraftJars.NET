using System.Text.Json.Serialization;

namespace MinecraftJars.Plugin.Fabric.Model.FabricApi;

public class Game
{
    [JsonPropertyName("version")] 
    public string Version { get; set; } = string.Empty;

    [JsonPropertyName("stable")]
    public bool Stable { get; set; }
}