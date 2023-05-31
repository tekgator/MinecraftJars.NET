![Pocketmine](Resources/Pocketmine-64px.png) Pocketmine plugin for MinecraftJar.NET
======

[Pocketmine](https://www.pocketmine.net/) provider plugin for [MinecraftJar.NET](../../README.md).

Provider for:
- Pocketmine

## Installing

The plugin is already bundled with the core library [MinecraftJar.NET](../../README.md).

## Additional information or steps for this plugin

The plugin has no specialities compared to the core interfaces.
If required the interface can be casted to it's instantiated classes.

- `IMinecraftProvider` to [PocketmineProvider](PocketmineProvider.cs)
- `IMinecraftProject` to [PocketmineProject](Model/PocketmineProject.cs)
- `IMinecraftVersion` to [PocketmineVersion](Model/PocketmineVersion.cs)
- `IMinecraftDownload` to [PocketmineDownload](Model/PocketmineDownload.cs)