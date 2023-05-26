using MinecraftJars.Core.Downloads;

namespace MinecraftJars.Plugin.Purpur.Model;

public record PurpurDownload(
    string FileName,
    long Size,
    string BuildId,
    string Url,
    DateTime? ReleaseTime,
    HashType HashType,
    string? Hash) : IDownload;