using MinecraftJars.Core.Versions;

namespace MinecraftJars.Core.Providers;

public interface IProvider
{
    ProviderOptions ProviderOptions { get; }
    string Name { get; }
    IEnumerable<string> AvailableGameTypes { get; }
    public Task<IEnumerable<IVersion>> GetVersions(
        VersionOptions? options = null, 
        CancellationToken cancellationToken = default!);
}
