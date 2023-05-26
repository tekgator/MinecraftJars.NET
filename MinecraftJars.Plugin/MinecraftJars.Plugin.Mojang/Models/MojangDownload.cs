using MinecraftJars.Core.Downloads;

namespace MinecraftJars.Plugin.Mojang.Models;

public record MojangDownload(
    string FileName,
    long Size,
    string BuildId,
    string Url,
    HashType HashType = HashType.None,
    string? Hash = null,
    DateTime? ReleaseTime = null) : IDownload;