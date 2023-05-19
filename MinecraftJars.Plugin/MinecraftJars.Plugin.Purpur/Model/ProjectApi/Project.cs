using System.Text.Json.Serialization;

namespace MinecraftJars.Plugin.Purpur.Model.ProjectApi;

#pragma warning disable CS8618
public class Project
{
    [JsonPropertyName("project")]
    public string ProjectName { get; set; }

    [JsonPropertyName("versions")]
    public List<string> Versions { get; set; }    
}
#pragma warning restore CS8618