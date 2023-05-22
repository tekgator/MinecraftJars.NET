using MinecraftJars.Core.Projects;
using MinecraftJars.Core.Versions;

namespace MinecraftJars.Core.Providers;

public interface IProvider
{
    ProviderOptions ProviderOptions { get; }
    string Name { get; }
    byte[] Logo { get; }
    IEnumerable<IProject> Projects { get; }
    public Task<IEnumerable<IVersion>> GetVersions(
        VersionOptions? options = null, 
        CancellationToken cancellationToken = default!);
}
