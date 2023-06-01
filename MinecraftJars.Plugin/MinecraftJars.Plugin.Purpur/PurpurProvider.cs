using System.ComponentModel.Composition;
using MinecraftJars.Core;
using MinecraftJars.Core.Projects;
using MinecraftJars.Core.Providers;
using MinecraftJars.Core.Versions;

namespace MinecraftJars.Plugin.Purpur;

[Export(typeof(IMinecraftProvider))]
public class PurpurProvider : IMinecraftProvider
{
    [ImportingConstructor]
    public PurpurProvider(
        PluginHttpClientFactory httpClientFactory, 
        ProviderOptions? options)
    {
        PurpurVersionFactory.HttpClientFactory = httpClientFactory;
        ProviderOptions = options ?? new ProviderOptions();
    }
    
    public ProviderOptions ProviderOptions { get; }
    public string Name => "Purpur";
    public byte[] Logo => Properties.Resources.Purpur;
    public IEnumerable<IMinecraftProject> Projects => PurpurProjectFactory.Projects;

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