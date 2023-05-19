using System.Text.Json.Serialization;

namespace MinecraftJars.Plugin.Paper.Model.BuildApi;

#pragma warning disable CS8618
public class Application
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("sha256")]
    public string Sha256 { get; set; }
}
#pragma warning restore CS8618