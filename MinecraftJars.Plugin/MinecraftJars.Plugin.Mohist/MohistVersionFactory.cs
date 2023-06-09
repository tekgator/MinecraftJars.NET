﻿using System.Net.Http.Json;
using MinecraftJars.Core;
using MinecraftJars.Core.Downloads;
using MinecraftJars.Core.Versions;
using MinecraftJars.Plugin.Mohist.Model;
using MinecraftJars.Plugin.Mohist.Model.MohistApi;

namespace MinecraftJars.Plugin.Mohist;

internal static class MohistVersionFactory
{
    private const string MohistVersionRequestUri = "https://mohistmc.com/api/versions";
    private const string MohistLatestBuildRequestUri = "https://mohistmc.com/api/{0}/latest/";

    public static PluginHttpClientFactory HttpClientFactory { get; set; } = default!;
    
    public static async Task<List<MohistVersion>> GetVersion(
        string projectName,
        VersionOptions options,
        CancellationToken cancellationToken)
    {
        var project = MohistProjectFactory.Projects.Single(p => p.Name.Equals(projectName));

        var client = HttpClientFactory.GetClient();
        var versionApi = await client.GetFromJsonAsync<List<string>>(MohistVersionRequestUri, cancellationToken) ??
            throw new InvalidOperationException("Could not acquire version details.");
    
        versionApi.Reverse();

        var versions = (from version in versionApi
            where string.IsNullOrWhiteSpace(options.Version) || version.Equals(options.Version)
            select new MohistVersion(
                Project: project,
                Version: version)
            ).ToList();
        
        return options.MaxRecords.HasValue 
            ? versions.Take(options.MaxRecords.Value).ToList() 
            : versions;
    }

    public static async Task<IMinecraftDownload> GetDownload(
        DownloadOptions options, 
        MohistVersion version, 
        CancellationToken cancellationToken)
    {
        var client = HttpClientFactory.GetClient();
        var requestUriLatestBuild = string.Format(MohistLatestBuildRequestUri, version.Version);
        var latestBuild = await client.GetFromJsonAsync<Build>(requestUriLatestBuild, cancellationToken);

        if (latestBuild == null || string.IsNullOrWhiteSpace(latestBuild.Url)) 
            throw new InvalidOperationException("Could not acquire download details.");

        long contentLength = 0;
        var fileName = $"{version.Project.Name}-{version.Version}-{latestBuild.Number}.jar";

        if (options.LoadFilesize)
        {
            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, latestBuild.Url);
            using var httpResponse = await client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            if (httpResponse.IsSuccessStatusCode)
            {
                contentLength = httpResponse.Content.Headers.ContentLength ?? contentLength;
                fileName = httpResponse.Content.Headers.ContentDisposition?.FileName ?? fileName;
            }
        }

        return new MohistDownload(
            FileName: fileName,
            Size: contentLength,
            BuildId: latestBuild.Number.ToString(),
            Url: latestBuild.Url,
            ReleaseTime: DateTimeOffset.FromUnixTimeMilliseconds(latestBuild.Timeinmillis).DateTime,
            HashType: HashType.Md5,
            Hash: latestBuild.Md5
        );
    }
}