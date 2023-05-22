using System.Diagnostics;
using MinecraftJars;
using MinecraftJars.Core.Projects;
using MinecraftJars.Core.Versions;

var providerManager = new ProviderManager();

foreach (var provider in providerManager.GetProviders())
{
    SetConsoleColor(ConsoleColor.White, ConsoleColor.Red);
    Console.WriteLine($"\n{provider.Name}:");
    ResetConsoleColor();

    foreach (var property in provider.GetType().GetProperties().Where(p => p.Name != "Name"))
    {
        Console.WriteLine($"\t{property.Name}: {property.GetValue(provider)}");
    }    
    
    SetConsoleColor(ConsoleColor.Green);
    Console.WriteLine("\n\tVersions:");
    ResetConsoleColor();
    
    foreach (var version in await provider.GetVersions())
    {
        SetConsoleColor(ConsoleColor.Magenta);
        Console.WriteLine($"\tVersion ID: {version.Version}");
        ResetConsoleColor();

        var download = await version.GetDownload();
        
        foreach (var property in version.GetType().GetProperties().Where(p => p.Name != "Version"))
        {
            Console.WriteLine($"\t\t{property.Name}: {property.GetValue(version)}");
        }
    }
}

static void SetConsoleColor(ConsoleColor foregroundColor, ConsoleColor? backgroundColor = null)
{
    Console.ForegroundColor = foregroundColor;
    if (backgroundColor is not null)
    {
        Console.BackgroundColor = (ConsoleColor)backgroundColor;
    }
}

static void ResetConsoleColor()
{
    Console.ForegroundColor = ConsoleColor.Gray;
    Console.BackgroundColor = ConsoleColor.Black;
}

Debugger.Break();