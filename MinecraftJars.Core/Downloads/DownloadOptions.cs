namespace MinecraftJars.Core.Downloads;

public class DownloadOptions
{
    /// <summary>
    /// Define if the plugin should use an online query to get the file size of the JAR if necessary
    /// Note: Can in increase load time for the plugin
    /// </summary>
    public bool LoadFilesize { get; set; } = true;
    
    /// <summary>
    /// In case the plugin needs to build the JAR instead of providing download information
    /// it can be configured whether the plugin should actually execute the build
    /// Note: It will increase execution time for the plugin dramatically
    /// </summary>    
    public bool BuildJar { get; set; } = false;

    /// <summary>
    /// If for the building purpose JAVA is required the path to the JAVA binary can be provided
    /// In case it is not provided the PATH environment variable will be utilized
    /// </summary>        
    public string JavaBin { get; set; } = "java";

    /// <summary>
    /// Optional callback function to show output of the build execution 
    /// </summary>        
    public Action<string>? BuildJarOutput;
}