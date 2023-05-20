using System.Text.Json.Serialization;

namespace MinecraftJars.Plugin.Paper.Model.BuildApi;

public class Application
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("sha256")]
    public string? Sha256 { get; set; }
}