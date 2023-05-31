using System.ComponentModel.Composition;
using MinecraftJars.Core.Projects;
using MinecraftJars.Core.Providers;
using MinecraftJars.Core.Versions;

namespace MinecraftJars.Plugin.Mojang;

[Export(typeof(IProvider))]
public class MojangProvider : IProvider
{
    [ImportingConstructor]
    public MojangProvider(ProviderOptions? options)
    {
        ProviderOptions = options ?? new ProviderOptions();
        MojangVersionFactory.HttpClient = ProviderOptions.GetHttpClient();
    }

    public ProviderOptions ProviderOptions { get; }
    public string Name => "Mojang";
    public byte[] Logo => Properties.Resources.Mojang;
    public IEnumerable<IProject> Projects => MojangProjectFactory.Projects;

    public async Task<IEnumerable<IVersion>> GetVersions(
        VersionOptions? options = null, 
        CancellationToken cancellationToken = default)
    {
        var versionOptions = options ?? new VersionOptions();
        var versions = new List<IVersion>();

        foreach (var project in Projects)
        {
            versions.AddRange(await project.GetVersions(versionOptions, cancellationToken));
        }

        return versions;
    }     
}