﻿using MinecraftJars.Core.Downloads;
using MinecraftJars.Core.Projects;
using MinecraftJars.Core.Versions;

namespace MinecraftJars.Plugin.Mohist.Model;

public record MohistVersion(
    IProject Project,
    string Version) : IVersion
{
    public Task<IDownload> GetDownload(DownloadOptions? options = null) => 
        MohistVersionFactory.GetDownload(options ?? new DownloadOptions(), this);
}
