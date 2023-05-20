using System.Text.Json.Serialization;

namespace MinecraftJars.Plugin.Purpur.Model.ProjectApi;

internal class Project
{
    [JsonPropertyName("project")]
    public string? ProjectName { get; set; }

    [JsonPropertyName("versions")]
    public List<string> Versions { get; set; } = new();
}