using System.Text.Json.Serialization;

namespace MinecraftJars.Core.Projects;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Group
{
    Server,
    Proxy,
    Bedrock
}