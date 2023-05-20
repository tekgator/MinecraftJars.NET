using System.Text.Json.Serialization;

namespace MinecraftJars.Core.Versions;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Group
{
    Server,
    Proxy,
    Bedrock
}