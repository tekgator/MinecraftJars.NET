![Paper](Resources/Paper-64px.png) Paper plugin for MinecraftJar.NET
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

- `IMinecraftProvider` to [PaperProvider](PaperProvider.cs)
- `IMinecraftProject` to [PaperProject](Model/PaperProject.cs)
- `IMinecraftVersion` to [PaperVersion](Model/PaperVersion.cs)
- `IMinecraftDownload` to [PaperDownload](Model/PaperDownload.cs)
