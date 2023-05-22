using System.ComponentModel.Composition;
using MinecraftJars.Core.Projects;
using MinecraftJars.Core.Providers;
using MinecraftJars.Core.Versions;

namespace MinecraftJars.Plugin.Purpur;

[Export(typeof(IProvider))]
public class PurpurProvider : IProvider
{
    [ImportingConstructor]
    public PurpurProvider(ProviderOptions? options)
    {
        ProviderOptions = options ?? new ProviderOptions();
    }
    
    public ProviderOptions ProviderOptions { get; }
    public string Name => Provider.Purpur;
    public byte[] Logo => Properties.Resources.Purpur;
    public IEnumerable<IProject> Projects => PurpurProjectFactory.Projects;

    public async Task<IEnumerable<IVersion>> GetVersions(VersionOptions? options = null, CancellationToken cancellationToken = default)
    {
        return await PurpurVersionFactory.Get(options ?? new VersionOptions(), cancellationToken);
    }
}