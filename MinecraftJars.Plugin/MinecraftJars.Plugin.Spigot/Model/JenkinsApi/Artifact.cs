using System.Text.Json.Serialization;

namespace MinecraftJars.Plugin.Spigot.Model.JenkinsApi;

internal class Artifact
{
    [JsonPropertyName("fileName")]
    public string FileName { get; set; } = string.Empty;

    [JsonPropertyName("relativePath")]
    public string RelativePath { get; set; } = string.Empty;
}