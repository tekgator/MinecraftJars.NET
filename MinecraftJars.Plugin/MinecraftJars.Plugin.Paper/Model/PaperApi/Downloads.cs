using System.Text.Json.Serialization;

namespace MinecraftJars.Plugin.Paper.Model.PaperApi;

public class Downloads
{
    [JsonPropertyName("application")]
    public Application Application { get; set; } = new();
}