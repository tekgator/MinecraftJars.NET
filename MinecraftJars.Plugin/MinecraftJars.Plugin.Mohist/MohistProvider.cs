using System.ComponentModel.Composition;
using MinecraftJars.Core.Projects;
using MinecraftJars.Core.Providers;
using MinecraftJars.Core.Versions;

namespace MinecraftJars.Plugin.Mohist;

[Export(typeof(IProvider))]
public class MohistProvider : IProvider
{
    [ImportingConstructor]
    public MohistProvider(ProviderOptions? options)
    {
        ProviderOptions = options ?? new ProviderOptions();
    }
    
    public ProviderOptions ProviderOptions { get; }
    public string Name => "MohistMC";
    public byte[] Logo => Properties.Resources.Mohist;
    public IEnumerable<IProject> Projects => MohistProjectFactory.Projects;
   
    public async Task<IEnumerable<IVersion>> GetVersions(VersionOptions? options = null, CancellationToken cancellationToken = default)
    {
        return await MohistVersionFactory.Get(options ?? new VersionOptions(), cancellationToken);
    }
}