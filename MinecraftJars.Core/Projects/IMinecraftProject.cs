using MinecraftJars.Core.Versions;

namespace MinecraftJars.Core.Projects;

public interface IMinecraftProject
{
    /// <summary>
    /// Group of the project e.g. Server, Bedrock or Proxy 
    /// </summary>        
    ProjectGroup ProjectGroup { get; }
    
    /// <summary>
    /// Name of the project e.g. Vanilla, Spigot, etc. 
    /// </summary>  
    string Name { get; }

    /// <summary>
    /// Short description of the project 
    /// </summary>  
    string Description { get; }

    /// <summary>
    /// Url of the project 
    /// </summary>    
    string Url { get; }

    /// <summary>
    /// Required runtime for the project
    /// </summary>
    ProjectRuntime ProjectRuntime { get; }
    
    /// <summary>
    /// PNG logo of the project 
    /// </summary>    
    byte[] Logo { get; }
    
    /// <summary>
    /// Get available versions for the project from the provider 
    /// </summary>    
    Task<IEnumerable<IMinecraftVersion>> GetVersions(
        VersionOptions? options = null, 
        CancellationToken cancellationToken = default!);     
}