![Fabric](Resources/Fabric-64px.png) Fabric plugin for MinecraftJar.NET
======

[Fabric](https://fabricmc.net/) provider plugin for [MinecraftJar.NET](../../README.md).

Provider for:
- Fabric

## Installing

The plugin is already bundled with the core library [MinecraftJar.NET](../../README.md).

## Additional information or steps for this plugin

The plugin has a few minor specialities compared to the core interfaces.
If required the interface can be casted to it's instantiated classes.

- `IMinecraftProvider` to [FabricProvider](FabricProvider.cs)
- `IMinecraftProject` to [FabricProject](Model/FabricProject.cs)
- `IMinecraftVersion` to [FabricVersion](Model/FabricVersion.cs)
- `IMinecraftDownload` to [FabricDownload](Model/FabricDownload.cs)
  - No `Hash`, `ReleaseTime` or `Size` is provided