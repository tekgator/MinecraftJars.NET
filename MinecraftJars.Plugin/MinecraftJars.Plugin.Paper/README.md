<img src="Resources/Paper.png" alt= “Pocketmine” width="64" height="64">
Paper plugin for MinecraftJar.NET
======

[Paper](https://papermc.io/) provider plugin for [MinecraftJar.NET](../../README.md).

Provider for:
- Paper
- Folia
- Velocity
- Waterfall

## Installing

The plugin is already bundled with the core library [MinecraftJar.NET](../../README.md).

## Additional information or steps for this plugin

The plugin has no specialities compared to the core interfaces.
If required the interface can be casted to it's instantiated classes.

- `IProvider` to [PaperProvider](PaperProvider.cs)
- `IProject` to [PaperProject](Model/PaperProject.cs)
- `IVersion` to [PaperVersion](Model/PaperVersion.cs)
- `IDownload` to [PaperDownload](Model/PaperDownload.cs)
