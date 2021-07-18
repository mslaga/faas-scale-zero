using System;
using System.Text.Json.Serialization;

namespace faasSzero.OpenFaaSApi.Model
{
    internal class ScaleRequest
    {
        [JsonPropertyName("serviceName")]
        public string ServiceName { get; set; }

        [JsonPropertyName("replicas")]
        public long Replicas { get; set; }

        public ScaleRequest(string serviceName, long replicas)
        {
            ServiceName = serviceName;
            Replicas = replicas;
        }

        public ScaleRequest()
        {
        }
    }
}
