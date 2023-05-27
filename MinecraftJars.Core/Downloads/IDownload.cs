namespace MinecraftJars.Core.Downloads;

public interface IDownload
{
    /// <summary>
    /// Filename of the download e.g. server-version.jar. Name could be used for the downloaded file 
    /// </summary>        
    string FileName { get; }

    /// <summary>
    /// Size of the download in bytes
    /// Note: May be 0 e.g. if option to query size online is turned off 
    /// </summary>        
    long Size { get; }

    /// <summary>
    /// Internal build ID of the version, if provided project does not have a build ID
    /// it will be filled with the Version 
    /// </summary>      
    string BuildId { get; }

    /// <summary>
    /// Download URL of the file
    /// Note: In case of a built JAR file this is a local URL (RequiresLocalBuild = true on IVersion interface)
    /// If local URL the user must make sure that the temp folder is removed from the file system 
    /// </summary>     
    string Url { get; }
    
    /// <summary>
    /// HashType of the provided Hash e.g. None, MD5, etc. 
    /// </summary>        
    HashType HashType { get; }

    /// <summary>
    /// Hash of the file.
    /// Note: If HashType is None no Hash has been provided by the provider 
    /// </summary>        
    string? Hash { get; }

    /// <summary>
    /// Release time of the built
    /// Note: Might not been provided by the provider 
    /// </summary>        
    DateTime? ReleaseTime { get; }
}