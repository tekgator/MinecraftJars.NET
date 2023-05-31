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
        PurpurVersionFactory.HttpClient = ProviderOptions.GetHttpClient();
    }
    
    public ProviderOptions ProviderOptions { get; }
    public string Name => "Purpur";
    public byte[] Logo => Properties.Resources.Purpur;
    public IEnumerable<IProject> Projects => PurpurProjectFactory.Projects;

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