using System.Text.Json.Serialization;

namespace MinecraftJars.Plugin.Mojang.Models.DetailApi;

#pragma warning disable CS8618
internal class JavaVersion
{
    [JsonPropertyName("component")]
    public string Component { get; set; }

    [JsonPropertyName("majorVersion")]
    public int MajorVersion { get; set; }
}
#pragma warning restore CS8618