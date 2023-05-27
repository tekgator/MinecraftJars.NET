using System.Text.Json.Serialization;

namespace MinecraftJars.Plugin.Spigot.Model.SpigotApi;

internal class SpigotBuild
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("toolsVersion")]
    public int ToolsVersion { get; set; }   
}