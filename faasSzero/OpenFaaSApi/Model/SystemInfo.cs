using System;
using System.Text.Json.Serialization;

namespace faasSzero.OpenFaaSApi.Model
{
    public class SystemInfo
    {
        public class VersionInfo
        {
            [JsonPropertyName("release")]
            public string Release { get; set; }
        }

        public class ProviderInfo
        {
            [JsonPropertyName("provider")]
            public string Provider { get; set; }

            [JsonPropertyName("orchestration")]
            public string Oorchestration { get; set; }

            [JsonPropertyName("version")]
            public VersionInfo Version;
        }
        [JsonPropertyName("provider")]
        public ProviderInfo Provider;

        [JsonPropertyName("version")]
        public VersionInfo Version;

        [JsonPropertyName("arch")]
        public string Arch { get; set; }

        public SystemInfo()
        {
        }
    }
}
