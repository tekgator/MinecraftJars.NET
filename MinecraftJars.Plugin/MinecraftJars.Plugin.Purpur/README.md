![Purpur](Resources/Purpur-64px.png) Purpur plugin for MinecraftJar.NET
======

[Purpur](https://purpurmc.org/) provider plugin for [MinecraftJar.NET](../../README.md).

Provider for:
- Purpur

## Installing

The plugin is already bundled with the core library [MinecraftJar.NET](../../README.md).

## Additional information or steps for this plugin

The plugin has no specialities compared to the core interfaces.
If required the interface can be casted to it's instantiated classes.

- `IMinecraftProvider` to [PurpurProvider](PurpurProvider.cs)
- `IMinecraftProject` to [PurpurProject](Model/PurpurProject.cs)
- `IMinecraftVersion` to [PurpurVersion](Model/PurpurVersion.cs)
- `IMinecraftDownload` to [PurpurDownload](Model/PurpurDownload.cs)
