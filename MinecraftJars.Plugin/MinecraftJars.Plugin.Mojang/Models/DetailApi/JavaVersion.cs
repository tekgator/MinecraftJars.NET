using System.Text.Json.Serialization;

namespace MinecraftJars.Plugin.Mojang.Models.DetailApi;

internal class JavaVersion
{
    [JsonPropertyName("component")]
    public string? Component { get; set; }

    [JsonPropertyName("majorVersion")]
    public int MajorVersion { get; set; }
}