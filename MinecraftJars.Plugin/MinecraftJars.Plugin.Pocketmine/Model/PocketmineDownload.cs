using MinecraftJars.Core.Downloads;

namespace MinecraftJars.Plugin.Pocketmine.Model;

public record PocketmineDownload(
    string FileName,
    long Size,
    string BuildId,
    string Url,
    DateTime? ReleaseTime,
    HashType HashType = HashType.None,
    string? Hash = null) : IMinecraftDownload;