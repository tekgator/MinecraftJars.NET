using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using MinecraftJars.Core;
using MinecraftJars.Core.Projects;
using MinecraftJars.Core.Providers;

namespace MinecraftJars;

public class MinecraftJarManager
{
    [ImportMany(typeof(IMinecraftProvider))]
    private IEnumerable<IMinecraftProvider> _providers;

    [Export]
    private ProviderOptions ProviderOptions { get; }
    
    [Export]
    private PluginHttpClientFactory HttpClientFactory { get; }
    
    public MinecraftJarManager(ProviderOptions? options = null)
    {
        ProviderOptions = options ?? new ProviderOptions();
        HttpClientFactory = new PluginHttpClientFactory(ProviderOptions.HttpClientFactory);
        
        var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".";
        var catalog = new AggregateCatalog();
        catalog.Catalogs.Add(new DirectoryCatalog(path));

        var container = new CompositionContainer(catalog);
        container.ComposeParts(this);

        _providers ??= Enumerable.Empty<IMinecraftProvider>();
    }
    
    /// <summary>
    /// Return a list of all providers (plugins)
    /// </summary>    
    public IEnumerable<IMinecraftProvider> GetProviders()
    {
        return _providers;
    }
    
    /// <summary>
    /// Return a list of all providers offering a certain project group
    /// </summary>    
    public IEnumerable<IMinecraftProvider> GetProviders(Group group)
    {
        var providers = new List<IMinecraftProvider>();

        providers.AddRange(GetProviders()
            .Where(p => p.Projects.Any(t => t.Group == group)));
        
        return providers;
    }    

    /// <summary>
    /// Return a specific provider
    /// </summary>    
    public IMinecraftProvider GetProvider(string provider)
    {
        return _providers
            .Single(p => p.Name.Equals(provider));
    }
    
    /// <summary>
    /// Return the provider for the provided Project
    /// </summary>    
    public IMinecraftProvider GetProvider(IMinecraftProject project)
    {
        return _providers
            .Single(p => p.Projects.Contains(project));
    }    

    /// <summary>
    /// Return a list of all projects (e.g. Vanilla, Spigot, etc.)
    /// </summary>       
    public IEnumerable<IMinecraftProject> GetProjects()
    {
        return GetProviders()
            .SelectMany(p => p.Projects);
    }
    
    /// <summary>
    /// Return a list of all projects for a certain type (e.g. all proxies)
    /// </summary>       
    public IEnumerable<IMinecraftProject> GetProjects(Group group)
    {
        return GetProviders()
            .SelectMany(p => p.Projects.Where(t => t.Group == group));
    }
}