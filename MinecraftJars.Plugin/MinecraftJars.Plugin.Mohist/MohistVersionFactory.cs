﻿using System.Net.Http.Json;
using System.Reflection;
using MinecraftJars.Core.Downloads;
using MinecraftJars.Core.Projects;
using MinecraftJars.Core.Versions;
using MinecraftJars.Plugin.Mohist.Model;
using MinecraftJars.Plugin.Mohist.Model.BuildApi;

namespace MinecraftJars.Plugin.Mohist;

internal static class MohistVersionFactory
{
    private const string MohistVersionRequestUri = "https://mohistmc.com/api/versions";
    private const string MohistLatestBuildRequestUri = "https://mohistmc.com/api/{0}/latest/";
    
    public static async Task<List<MohistVersion>> Get(
        VersionOptions options,
        CancellationToken cancellationToken = default!)
    {
        var versions = new List<MohistVersion>();
        var projects = new List<MohistProject>(MohistProjectFactory.Projects);

        if (!string.IsNullOrWhiteSpace(options.ProjectName))
            projects.RemoveAll(t => !t.Name.Equals(options.ProjectName));

        if (!projects.Any() || (options.Group is not null && options.Group is not Group.Server))
            return versions;
        
        using var client = GetHttpClient();

        foreach (var project in projects)
        {
            var availVersions = await client.GetFromJsonAsync<List<string>>(MohistVersionRequestUri, cancellationToken);        
        
            if (availVersions == null) 
                throw new InvalidOperationException("Could not acquire game type details.");
       
            if (options.Version is not null)
                availVersions.RemoveAll(v => !v.Equals(options.Version));
            
            availVersions.Reverse();

            versions.AddRange(availVersions
                .Select(version => new MohistVersion(
                    Project: project,
                    Version: version 
                )));
        }
        
        return versions;
    }

    public static async Task<IDownload> GetDownload(MohistVersion version)
    {
        using var client = GetHttpClient();
        
        var requestUriLatestBuild = string.Format(MohistLatestBuildRequestUri, version.Version);
        var latestBuild = await client.GetFromJsonAsync<Build>(requestUriLatestBuild);

        if (latestBuild == null || string.IsNullOrWhiteSpace(latestBuild.Url)) 
            throw new InvalidOperationException("Could not acquire download details.");
        
        using var requestMessage = new HttpRequestMessage(HttpMethod.Get, latestBuild.Url);
        using var httpResponse = await client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);

        long contentLength = 0;
        var fileName = $"{version.Project.Group.ToString().ToLower()}-{version.Version}-{latestBuild.Number}.jar";
        if (httpResponse.IsSuccessStatusCode)
        {
            contentLength = httpResponse.Content.Headers.ContentLength ?? contentLength;
            fileName = httpResponse.Content.Headers.ContentDisposition?.FileName ?? fileName;
        }

        return new MohistDownload(
            FileName: fileName,
            Size: contentLength,
            BuildId: latestBuild.Number,
            Url: latestBuild.Url,
            ReleaseTime: DateTimeOffset.FromUnixTimeMilliseconds(latestBuild.Timeinmillis).DateTime,
            HashType: HashType.Md5,
            Hash: latestBuild.Md5
        );
    }      
    
    private static HttpClient GetHttpClient()
    {
        var client = new HttpClient();

        if (client.DefaultRequestHeaders.UserAgent.Any())
            return client;

        var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
        client.DefaultRequestHeaders.UserAgent.TryParseAdd(assembly.GetName().Name);

        return client;
    }
}