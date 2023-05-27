using System.Diagnostics;
using MinecraftJars;
using MinecraftJars.Core.Versions;

var providerManager = new ProviderManager();

foreach (var provider in providerManager.GetProviders())
{
    SetConsoleColor(ConsoleColor.White, ConsoleColor.Red);
    Console.WriteLine($"{provider.Name}:");
    ResetConsoleColor();

    foreach (var project in provider.Projects)
    {
        Console.WriteLine($"\t{project}");     
        
        foreach (var version in await provider.GetVersions(project.Name, new VersionOptions { MaxRecords = 10}))
        {
            Console.WriteLine($"\t\t{version}");

            var download = await version.GetDownload();
            Console.WriteLine($"\t\t\t{download}");
        }
        
        Console.WriteLine();
    }
    
    Console.WriteLine();
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