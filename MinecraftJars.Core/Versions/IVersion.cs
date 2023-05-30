using MinecraftJars.Core.Downloads;
using MinecraftJars.Core.Projects;

namespace MinecraftJars.Core.Versions;

public interface IVersion
{
    /// <summary>
    /// Project information about the version 
    /// </summary>        
    IProject Project { get; }

    /// <summary>
    /// Version name e.g. 1.19.4 
    /// </summary>        
    string Version { get; }

    /// <summary>
    /// Indicates whether this version experimental 
    /// </summary>      
    bool IsSnapShot { get; }
    
    /// <summary>
    /// Indicates TRUE in case the plugin needs to build the JAR file instead of providing download information
    /// </summary>        
    bool RequiresLocalBuild => false;

    /// <summary>
    /// Get the download information of the version
    /// Note: In case this method is building the actual JAR the user is responsible to move/delete the temp.
    /// directory including the built JAR file 
    /// </summary>    
    Task<IDownload> GetDownload(DownloadOptions? options = null, CancellationToken cancellationToken = default!);
}
