using Newtonsoft.Json;

namespace H.NET.Converters
{
    internal sealed class WitAiResponse
    {
        [JsonProperty("text")]
        public string? Text { get; set; }
    }
}