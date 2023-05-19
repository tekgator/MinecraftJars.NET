using System.Text.Json.Serialization;

namespace MinecraftJars.Plugin.Paper.Model.BuildApi;

#pragma warning disable CS8618
public class Downloads
{
    [JsonPropertyName("application")]
    public Application Application { get; set; }
}
#pragma warning restore CS8618