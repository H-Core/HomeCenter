using Newtonsoft.Json;

namespace H.NET.Converters
{
    internal class WitAiResponse
    {
        [JsonProperty("_text")]
        public string? Text { get; set; }

        [JsonProperty("entities")]
        public object? Entities { get; set; }

        [JsonProperty("msg_id")]
        public string? MsgId { get; set; }
    }
}