using System.Text.Json.Serialization;

namespace MinecraftJars.Plugin.Pocketmine.Model.ReleaseApi;

internal class Release
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("tag_name")]
    public string TagName { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("draft")]
    public bool Draft { get; set; }

    [JsonPropertyName("prerelease")]
    public bool Prerelease { get; set; }

    [JsonPropertyName("published_at")]
    public DateTime PublishedAt { get; set; }

    [JsonPropertyName("assets")] 
    public List<Asset> Assets { get; set; } = new();
}