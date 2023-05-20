using System.Text.Json.Serialization;

namespace MinecraftJars.Plugin.Paper.Model.BuildApi;

public class Downloads
{
    [JsonPropertyName("application")]
    public Application Application { get; set; } = new();
}