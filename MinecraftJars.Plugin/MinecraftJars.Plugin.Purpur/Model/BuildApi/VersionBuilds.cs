using System.Text.Json.Serialization;

namespace MinecraftJars.Plugin.Purpur.Model.BuildApi;

#pragma warning disable CS8618
public class VersionBuilds
{
    [JsonPropertyName("builds")]
    public Builds Builds { get; set; }

    [JsonPropertyName("project")]
    public string Project { get; set; }

    [JsonPropertyName("version")]
    public string Version { get; set; }    
}
#pragma warning restore CS8618