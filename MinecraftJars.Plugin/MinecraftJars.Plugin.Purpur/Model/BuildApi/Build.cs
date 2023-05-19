using System.Text.Json.Serialization;

namespace MinecraftJars.Plugin.Purpur.Model.BuildApi;

#pragma warning disable CS8618
public class Build
{
    [JsonPropertyName("build")]
    public string BuildId { get; set; }

    [JsonPropertyName("duration")]
    public int Duration { get; set; }

    [JsonPropertyName("md5")]
    public string Md5 { get; set; }

    [JsonPropertyName("project")]
    public string Project { get; set; }

    [JsonPropertyName("result")]
    public string Result { get; set; }

    [JsonPropertyName("timestamp")]
    public long Timestamp { get; set; }

    [JsonPropertyName("version")]
    public string Version { get; set; }    
}
#pragma warning restore CS8618