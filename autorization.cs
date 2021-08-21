using System;
using System.Text.Json.Serialization;

namespace WebAPIClient
{
    public class Token
    {
        [JsonPropertyName("accessToken")]
        public string AccessToken {get; set;}

        public Token(){

        }

    }
}