using System.Text.Json.Serialization;

namespace MinecraftJars.Plugin.Paper.Model.ProjectApi;

#pragma warning disable CS8618
internal class Project
{
    [JsonPropertyName("project_id")]
    public string ProjectId { get; set; }

    [JsonPropertyName("project_name")]
    public string ProjectName { get; set; }

    [JsonPropertyName("version_groups")]
    public List<string> VersionGroups { get; set; }

    [JsonPropertyName("versions")]
    public List<string> Versions { get; set; }    
}
#pragma warning restore CS8618