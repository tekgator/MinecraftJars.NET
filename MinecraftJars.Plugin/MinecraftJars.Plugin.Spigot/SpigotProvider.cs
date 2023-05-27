using System.ComponentModel.Composition;
using MinecraftJars.Core.Projects;
using MinecraftJars.Core.Providers;
using MinecraftJars.Core.Versions;

namespace MinecraftJars.Plugin.Spigot;

[Export(typeof(IProvider))]
public class SpigotProvider : IProvider
{
    [ImportingConstructor]
    public SpigotProvider(ProviderOptions? options)
    {
        ProviderOptions = options ?? new ProviderOptions();
        SpigotVersionFactory.HttpClientFactory = ProviderOptions.HttpClientFactory;
    }

    public ProviderOptions ProviderOptions { get; }
    public string Name => "SpigotMC";
    public byte[] Logo => Properties.Resources.Spigot;
    public IEnumerable<IProject> Projects => SpigotProjectFactory.Projects;
    
    public async Task<IEnumerable<IVersion>> GetVersions(VersionOptions? options = null, CancellationToken cancellationToken = default)
    {
        return await SpigotVersionFactory.GetVersion(options ?? new VersionOptions(), cancellationToken);
    }
}