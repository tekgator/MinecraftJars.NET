using System.Text.Json.Serialization;

namespace MinecraftJars.Plugin.Purpur.Model.PurpurApi;

internal class Project
{
    [JsonPropertyName("project")]
    public string? ProjectName { get; set; }

    [JsonPropertyName("versions")]
    public List<string> Versions { get; set; } = new();
}