using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using MinecraftJars.Core.Projects;
using MinecraftJars.Core.Providers;

namespace MinecraftJars;

public class ProviderManager
{
    [ImportMany(typeof(IProvider))]
    private IEnumerable<IProvider> _providers;

    [Export]
    private ProviderOptions ProviderOptions { get; }
    
    public ProviderManager(ProviderOptions? options = null)
    {
        ProviderOptions = options ?? new ProviderOptions();
        
        var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".";
        var catalog = new AggregateCatalog();
        catalog.Catalogs.Add(new DirectoryCatalog(path));

        var container = new CompositionContainer(catalog);
        container.ComposeParts(this);

        _providers ??= Enumerable.Empty<IProvider>();
    }
    
    /// <summary>
    /// Return a list of all providers (plugins)
    /// </summary>    
    public IEnumerable<IProvider> GetProviders()
    {
        return _providers;
    }
    
    /// <summary>
    /// Return a list of all providers offering a certain project group
    /// </summary>    
    public IEnumerable<IProvider> GetProviders(Group group)
    {
        var providers = new List<IProvider>();

        providers.AddRange(GetProviders()
            .Where(p => p.Projects.Any(t => t.Group == group)));
        
        return providers;
    }    

    /// <summary>
    /// Return a specific provider
    /// </summary>    
    public IProvider GetProvider(string provider)
    {
        return _providers
            .Single(p => p.Name.Equals(provider));
    }
    
    /// <summary>
    /// Return the provider for the provided Project
    /// </summary>    
    public IProvider GetProvider(IProject project)
    {
        return _providers
            .Single(p => p.Projects.Contains(project));
    }    

    /// <summary>
    /// Return a list of all projects (e.g. Vanilla, Spigot, etc.)
    /// </summary>       
    public IEnumerable<IProject> GetProjects()
    {
        return GetProviders()
            .SelectMany(p => p.Projects);
    }
    
    /// <summary>
    /// Return a list of all projects for a certain type (e.g. all proxies)
    /// </summary>       
    public IEnumerable<IProject> GetProjects(Group group)
    {
        return GetProviders()
            .SelectMany(p => p.Projects.Where(t => t.Group == group));
    }
}