<img src="Resources/Spigot.png" alt= “Spigot” width="64" height="64"> Spigot plugin for MinecraftJar.NET
======

[Spigot](https://www.spigotmc.org/) provider plugin for [MinecraftJar.NET](../../README.md).

Provider for:
- Spigot
- BungeeCoord

## Installing

The plugin is already bundled with the core library [MinecraftJar.NET](../../README.md).

## Additional information or steps for this plugin

The plugin has a few minor specialities compared to the core interfaces.
If required the interface can be casted to it's instantiated classes.

- `IProvider` to [SpigotProvider](SpigotProvider.cs)
- `IProject` to [SpigotProject](Model/SpigotProject.cs)
- `IVersion` to [SpigotVersion](Model/SpigotVersion.cs)
  - For Spigot `RequiresLocalBuild` is always true, see notes below
  - For BungeeCord `Version` is the BuildId as BungeeCord isn't based on versions
  - For Spigot the `ReleaseTime` (for older versions?) doesn't seem to be accurate resulting in an unsorted version list
- `IDownload` to [SpigotDownload](Model/SpigotDownload.cs)
  - For all groups, no `Hash` is provided
  - For Spigot the `ReleaseTime` (for older versions?) doesn't seem to be accurate
  - For Spigot the `Url` is a local built file


## Spigot specialities

As Spigot cannot be downloaded it must be build locally. To indicate that a build is required the `IVersion.RequiresLocalBuild` property is always set to true for Spigot.
When calling `IVersion.GetDownload` [DownloadOptions](../../MinecraftJars.Core/Downloads/DownloadOptions.cs) with `BuildJar = true` must be supplied to start a local build.
The actual build is fairly time consuming depending on the machine.

Please make sure following tools are installed:
- Git (not required for Windows --> will be downloaded automatically as a portable version)
- Java 17 or higher 

For details concerning the build process please find information on the [Spigot BuildTools Website](https://www.spigotmc.org/wiki/buildtools/) as well.

The Url returned in the `IDownload` object is the local build file in a local temporary directory. 
After moving the Jar file to its destination the user/developer is responsible to remove the temporary directory.