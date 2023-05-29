<img src="Resources/Mohist.png" alt= “Pocketmine” width="64" height="64">
Mohist plugin for MinecraftJar.NET
======

[Mohist](https://mohistmc.com/) provider plugin for [MinecraftJar.NET](../../README.md).

Provider for:
- Mohist

## Installing

The plugin is already bundled with the core library [MinecraftJar.NET](../../README.md).

## Additional information or steps for this plugin

The plugin has no specialities compared to the core interfaces.
If required the interface can be casted to it's instantiated classes.

- `IProvider` to [MohistProvider](MohistProvider.cs)
- `IProject` to [MohistProject](Model/MohistProject.cs)
- `IVersion` to [MohistVersion](Model/MohistVersion.cs)
- `IDownload` to [MohistDownload](Model/MohistDownload.cs)