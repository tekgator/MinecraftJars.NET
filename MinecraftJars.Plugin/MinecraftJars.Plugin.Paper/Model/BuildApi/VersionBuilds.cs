using System.Text.Json.Serialization;

namespace MinecraftJars.Plugin.Paper.Model.BuildApi;

#pragma warning disable CS8618
internal class BuildVersions
{
    [JsonPropertyName("project_id")]
    public string ProjectId { get; set; }

    [JsonPropertyName("project_name")]
    public string ProjectName { get; set; }

    [JsonPropertyName("version")]
    public string Version { get; set; }

    [JsonPropertyName("builds")]
    public List<Build> Builds { get; set; }    
}
#pragma warning restore CS8618