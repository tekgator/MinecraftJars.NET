namespace MinecraftJars.Core.Projects;

public interface IProject
{
    /// <summary>
    /// Group of the project e.g. Server, Bedrock or Proxy 
    /// </summary>        
    Group Group { get; }
    
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
    /// PNG logo of the project 
    /// </summary>    
    byte[] Logo { get; }
}