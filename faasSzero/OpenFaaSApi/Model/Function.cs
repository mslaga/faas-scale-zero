using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace faasSzero.OpenFaaSApi.Model
{
    public class Function
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("image")]
        public string Image { get; set; }

        [JsonPropertyName("namespace")]
        public string Namespace {get;set; }

        [JsonPropertyName("envProcess")]
        public string EnvProcess { get; set; }

        [JsonPropertyName("envVars")]
        public Dictionary<string, string> EnvVars { get; set; }

        [JsonPropertyName("labels")]
        public Dictionary<string, string> Labels { get; set; }

        [JsonPropertyName("annotations")]
        public Dictionary<string, string> Annotations { get; set; }

        [JsonPropertyName("replicas")]
        public long Replicas { get; set; }

        [JsonPropertyName("availableReplicas")]
        public long AvailableReplicas { get; set; }

        [JsonPropertyName("invocationCount")]
        public long InvocationCount { get; set; }

        public Function()
        {
        }
    }
}
