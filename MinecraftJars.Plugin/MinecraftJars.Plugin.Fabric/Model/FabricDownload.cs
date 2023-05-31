using MinecraftJars.Core.Downloads;

namespace MinecraftJars.Plugin.Fabric.Model;

public record FabricDownload(
    string FileName,
    long Size,
    string BuildId,
    string Url,
    DateTime? ReleaseTime = null,
    HashType HashType = HashType.None,
    string? Hash = null) : IMinecraftDownload;