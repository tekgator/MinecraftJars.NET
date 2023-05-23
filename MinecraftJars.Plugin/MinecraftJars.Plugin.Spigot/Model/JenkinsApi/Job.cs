using System.Text.Json.Serialization;

namespace MinecraftJars.Plugin.Spigot.Model.JenkinsApi;

internal class Job
{
    [JsonPropertyName("builds")]
    public List<Build> Builds { get; set; } = new();
}

