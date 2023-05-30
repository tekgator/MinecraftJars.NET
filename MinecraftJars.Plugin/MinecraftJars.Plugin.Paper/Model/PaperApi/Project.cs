using System.Text.Json.Serialization;

namespace MinecraftJars.Plugin.Paper.Model.PaperApi;

internal class Project
{
    [JsonPropertyName("project_id")]
    public string? ProjectId { get; set; }

    [JsonPropertyName("project_name")]
    public string? ProjectName { get; set; }

    [JsonPropertyName("version_groups")]
    public List<string> VersionGroups { get; set; } = new();

    [JsonPropertyName("versions")]
    public List<string> Versions { get; set; } = new();  
}