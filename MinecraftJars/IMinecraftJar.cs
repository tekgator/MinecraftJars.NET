using MinecraftJars.Core.Projects;
using MinecraftJars.Core.Providers;

namespace MinecraftJars;

public interface IMinecraftJar
{
    /// <summary>
    /// Return a list of all providers (plugins)
    /// </summary>    
    IEnumerable<IMinecraftProvider> GetProviders();

    /// <summary>
    /// Return a list of all providers offering a certain project group
    /// </summary>    
    IEnumerable<IMinecraftProvider> GetProviders(Group group);

    /// <summary>
    /// Return a specific provider
    /// </summary>    
    IMinecraftProvider GetProvider(string provider);

    /// <summary>
    /// Return the provider for the provided Project
    /// </summary>    
    IMinecraftProvider GetProvider(IMinecraftProject project);

    /// <summary>
    /// Return a list of all projects (e.g. Vanilla, Spigot, etc.)
    /// </summary>       
    IEnumerable<IMinecraftProject> GetProjects();

    /// <summary>
    /// Return a list of all projects for a certain type (e.g. all proxies)
    /// </summary>       
    IEnumerable<IMinecraftProject> GetProjects(Group group);
}