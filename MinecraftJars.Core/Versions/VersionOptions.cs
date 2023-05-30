using MinecraftJars.Core.Projects;

namespace MinecraftJars.Core.Versions;

public class VersionOptions
{
    /// <summary>
    /// Filter for a specific version 
    /// </summary>        
    public string? Version { get; set; }

    /// <summary>
    /// Indicates to return snapshot / experimental / preview / beta / alpha builds as well 
    /// </summary>       
    public bool IncludeSnapshotBuilds { get; set; }
    
    /// <summary>
    /// Limit the amount of records returned by the plugin API. Useful if only
    /// e.g. the last x Versions are required.
    /// Note: can increase performance of certain plugins 
    /// </summary>        
    public int? MaxRecords { get; set; }
}