﻿using System.Text.Json.Serialization;

namespace MinecraftJars.Plugin.Paper.Model.BuildApi;

#pragma warning disable CS8618
public class Build
{
    [JsonPropertyName("build")]
    public int BuildId { get; set; }

    [JsonPropertyName("time")]
    public DateTime Time { get; set; }

    [JsonPropertyName("channel")]
    public string Channel { get; set; }

    [JsonPropertyName("promoted")]
    public bool Promoted { get; set; }

    [JsonPropertyName("downloads")]
    public Downloads Downloads { get; set; }
}    
#pragma warning restore CS8618