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
    private PluginHttpClientFactory HttpClientFactory { get; }

    public MinecraftJar(MinecraftJarOptions? options = null)
    {
        HttpClientFactory = new PluginHttpClientFactory(options?.HttpClientFactory);
        
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
    
    public IEnumerable<IMinecraftProvider> GetProviders(ProjectGroup projectGroup)
    {
        return from provider in GetProviders()
            from project in provider.Projects
            where project.ProjectGroup == projectGroup
            select provider;
    }    

    public IMinecraftProvider? GetProvider(string providerName)
    {
        return (from provider in GetProviders()
            where provider.Name.Equals(providerName)
            select provider).SingleOrDefault();
    }
    
    public IMinecraftProvider? GetProvider(IMinecraftProject project)
    {
        return (from provider in GetProviders()
            where provider.Projects.Contains(project)
            select provider).SingleOrDefault();
    }    

    public IEnumerable<IMinecraftProject> GetProjects()
    {
        return from provider in GetProviders()
            from project in provider.Projects
            select project;
    }
    
    public IEnumerable<IMinecraftProject> GetProjects(ProjectGroup projectGroup)
    {
        return from provider in GetProviders()
            from project in provider.Projects
            where project.ProjectGroup == projectGroup
            select project;
    }

    public IMinecraftProject? GetProject(string projectName)
    {
        return (from provider in GetProviders()
            from project in provider.Projects
            where project.Name.Equals(projectName)
            select project).SingleOrDefault();
    }
}