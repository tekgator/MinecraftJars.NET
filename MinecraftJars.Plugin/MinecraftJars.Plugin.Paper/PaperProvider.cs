using System.ComponentModel.Composition;
using MinecraftJars.Core.Projects;
using MinecraftJars.Core.Providers;
using MinecraftJars.Core.Versions;

namespace MinecraftJars.Plugin.Paper;

[Export(typeof(IProvider))]
public class PaperProvider : IProvider
{
    [ImportingConstructor]
    public PaperProvider(ProviderOptions? options)
    {
        ProviderOptions = options ?? new ProviderOptions();
        PaperVersionFactory.HttpClientFactory = ProviderOptions.HttpClientFactory;
    }
    
    public ProviderOptions ProviderOptions { get; }
    public string Name => "PaperMC";
    public byte[] Logo => Properties.Resources.Paper;
    public IEnumerable<IProject> Projects => PaperProjectFactory.Projects;

    public async Task<IEnumerable<IVersion>> GetVersions(VersionOptions? options = null, CancellationToken cancellationToken = default)
    {
        return await PaperVersionFactory.GetVersion(options ?? new VersionOptions(), cancellationToken);
    }
}