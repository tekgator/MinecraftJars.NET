using System.Text.Json.Serialization;

namespace MinecraftJars.Plugin.Mojang.Models.DetailApi;

#pragma warning disable CS8618
internal class Server
{
    [JsonPropertyName("sha1")]
    public string Sha1 { get; set; }

    [JsonPropertyName("size")]
    public int Size { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }    
}
#pragma warning restore CS8618