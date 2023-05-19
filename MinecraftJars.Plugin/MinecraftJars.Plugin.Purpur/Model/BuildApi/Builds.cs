using System.Text.Json.Serialization;

namespace MinecraftJars.Plugin.Purpur.Model.BuildApi;

#pragma warning disable CS8618
public class Builds
{
    [JsonPropertyName("all")]
    public List<string> All { get; set; }

    [JsonPropertyName("latest")]
    public string Latest { get; set; }    
}
#pragma warning restore CS8618