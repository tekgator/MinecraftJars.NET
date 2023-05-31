using System.ComponentModel.Composition;
using MinecraftJars.Core.Projects;
using MinecraftJars.Core.Providers;
using MinecraftJars.Core.Versions;

namespace MinecraftJars.Plugin.Paper;

[Export(typeof(IMinecraftProvider))]
public class PaperProvider : IMinecraftProvider
{
    [ImportingConstructor]
    public PaperProvider(ProviderOptions? options)
    {
        ProviderOptions = options ?? new ProviderOptions();
        PaperVersionFactory.HttpClient = ProviderOptions.GetHttpClient();
    }
    
    public ProviderOptions ProviderOptions { get; }
    public string Name => "Paper";
    public byte[] Logo => Properties.Resources.Paper;
    public IEnumerable<IMinecraftProject> Projects => PaperProjectFactory.Projects;

    public async Task<IEnumerable<IMinecraftVersion>> GetVersions(
        VersionOptions? options = null, 
        CancellationToken cancellationToken = default)
    {
        var versionOptions = options ?? new VersionOptions();
        var versions = new List<IMinecraftVersion>();

        foreach (var project in Projects)
        {
            versions.AddRange(await project.GetVersions(versionOptions, cancellationToken));
        }

        return versions;
    }
}