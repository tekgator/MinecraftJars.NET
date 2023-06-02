using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using MinecraftJars.Core;
using MinecraftJars.Core.Projects;
using MinecraftJars.Core.Providers;

namespace MinecraftJars;

public class MinecraftJar : IMinecraftJar
{
    [ImportMany(typeof(IMinecraftProvider))]
    private IEnumerable<IMinecraftProvider> _providers;

    [Export]
    private ProviderOptions ProviderOptions { get; }
    
    [Export]
    private PluginHttpClientFactory HttpClientFactory { get; }
    
    public MinecraftJar(ProviderOptions? options = null)
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
    
    public IEnumerable<IMinecraftProvider> GetProviders()
    {
        return _providers;
    }
    
    public IEnumerable<IMinecraftProvider> GetProviders(Group group)
    {
        var providers = new List<IMinecraftProvider>();

        providers.AddRange(GetProviders()
            .Where(p => p.Projects.Any(t => t.Group == group)));
        
        return providers;
    }    

    public IMinecraftProvider GetProvider(string provider)
    {
        return _providers
            .Single(p => p.Name.Equals(provider));
    }
    
    public IMinecraftProvider GetProvider(IMinecraftProject project)
    {
        return _providers
            .Single(p => p.Projects.Contains(project));
    }    

    public IEnumerable<IMinecraftProject> GetProjects()
    {
        return GetProviders()
            .SelectMany(p => p.Projects);
    }
    
    public IEnumerable<IMinecraftProject> GetProjects(Group group)
    {
        return GetProviders()
            .SelectMany(p => p.Projects.Where(t => t.Group == group));
    }
}