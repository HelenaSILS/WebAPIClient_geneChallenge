using System;
using System.Text.Json.Serialization;

namespace WebAPIClient
{
    public class Job
    {
        [JsonPropertyName("id")]
        public string Id {get; set;}

        public string Path{get;set;}

        [JsonPropertyName("type")]
        public string Type {get; set;}

        [JsonPropertyName("strand")]
        public string Strand {get; set;}

        [JsonPropertyName("strandEncoded")]
        public string StrandEncoded {get; set;}

        [JsonPropertyName("geneEncoded")]
        public string GeneEncoded {get; set;}

        public Job(){

        }
    }
}
