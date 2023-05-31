using MinecraftJars.Core.Projects;
using MinecraftJars.Core.Versions;

namespace MinecraftJars.Core.Providers;

public interface IProvider
{
    /// <summary>
    /// The options the provider manager has been provided with 
    /// </summary>        
    ProviderOptions ProviderOptions { get; }

    /// <summary>
    /// Name of the provider e.g. Mojang, PaperMC, etc. 
    /// </summary>        
    string Name { get; }

    /// <summary>
    /// PNG logo of the Provider 
    /// </summary>    
    byte[] Logo { get; }

    /// <summary>
    /// Available projects of the provider 
    /// </summary>    
    IEnumerable<IProject> Projects { get; }

    /// <summary>
    /// Get available versions for all projects from the provider 
    /// </summary>    
    public Task<IEnumerable<IVersion>> GetVersions(
        VersionOptions? options = null, 
        CancellationToken cancellationToken = default!);
}
