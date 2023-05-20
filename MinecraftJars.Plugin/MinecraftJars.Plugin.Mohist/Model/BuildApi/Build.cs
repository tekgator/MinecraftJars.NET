using System.Text.Json.Serialization;

namespace MinecraftJars.Plugin.Mohist.Model.BuildApi;

internal class Build
{
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("number")]
    public int Number { get; set; }

    [JsonPropertyName("version")]
    public string? Version { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("forge_version")]
    public string? ForgeVersion { get; set; }

    [JsonPropertyName("tinysha")]
    public string? Tinysha { get; set; }

    [JsonPropertyName("fullsha")]
    public string? Fullsha { get; set; }

    [JsonPropertyName("md5")]
    public string? Md5 { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("mirror")]
    public string? Mirror { get; set; }

    [JsonPropertyName("timeinmillis")]
    public long Timeinmillis { get; set; }

    [JsonPropertyName("date")]
    public string? Date { get; set; }
}