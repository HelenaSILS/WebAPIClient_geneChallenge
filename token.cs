using System;
using System.Text.Json.Serialization;

namespace WebAPIClient
{
    public class TokenAccess
    {
        [JsonPropertyName("accessToken")]
        public string AccessToken {get; set;}

        public TokenAccess(){

        }

    }
}
