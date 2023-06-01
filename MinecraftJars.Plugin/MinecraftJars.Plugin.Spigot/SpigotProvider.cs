using System.ComponentModel.Composition;
using MinecraftJars.Core;
using MinecraftJars.Core.Projects;
using MinecraftJars.Core.Providers;
using MinecraftJars.Core.Versions;

namespace MinecraftJars.Plugin.Spigot;

[Export(typeof(IMinecraftProvider))]
public class SpigotProvider : IMinecraftProvider
{
    [ImportingConstructor]
    public SpigotProvider(
        PluginHttpClientFactory httpClientFactory, 
        ProviderOptions? options)
    {
        SpigotVersionFactory.HttpClientFactory = httpClientFactory;
        ProviderOptions = options ?? new ProviderOptions();
    }
    
    public ProviderOptions ProviderOptions { get; }
    public string Name => "Spigot";
    public byte[] Logo => Properties.Resources.Spigot;
    public IEnumerable<IMinecraftProject> Projects => SpigotProjectFactory.Projects;
    
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