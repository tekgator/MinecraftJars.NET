![Mohist](Resources/Mohist-64px.png) Mohist plugin for MinecraftJar.NET
======

[Mohist](https://mohistmc.com/) provider plugin for [MinecraftJar.NET](../../README.md).

Provider for:
- Mohist

## Installing

The plugin is already bundled with the core library [MinecraftJar.NET](../../README.md).

## Additional information or steps for this plugin

The plugin has no specialities compared to the core interfaces.
If required the interface can be casted to it's instantiated classes.

- `IMinecraftProvider` to [MohistProvider](MohistProvider.cs)
- `IMinecraftProject` to [MohistProject](Model/MohistProject.cs)
- `IMinecraftVersion` to [MohistVersion](Model/MohistVersion.cs)
- `IMinecraftDownload` to [MohistDownload](Model/MohistDownload.cs)