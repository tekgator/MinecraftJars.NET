Mojang plugin for MinecraftJar.NET
======

[Mojang](https://www.minecraft.net/) provider plugin for [MinecraftJar.NET](../../README.md).

Provider for:
- Vanilla
- Vanilla Preview
- Bedrock
- Bedrock Preview

## Installing

The plugin is already bundled with the core library [MinecraftJar.NET](../../README.md).

## Additional information or steps for this plugin

The plugin has a few minor specialities compared to the core interfaces.
If required the interface can be casted to it's instantiated classes.

- `IProvider` to [MojangProvider](MojangProvider.cs)
- `IProject` to [MojangProject](Model/MojangProject.cs)
- `IVersion` to [MojangVersion](Model/MojangVersion.cs)
  - For all groups the Build ID is the same as the version ID
  - For Bedrock an `Os` property is provided as the executables are compiled for Windows or Linux
- `IDownload` to [MojangDownload](Model/MojangDownload.cs)
  - For Bedrock, no `Hash` or `ReleaseTime` is provided
  - Older version did not provide as server therefore `Url` might be empty