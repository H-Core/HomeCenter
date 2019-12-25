using System.Collections.Generic;
using Newtonsoft.Json;

namespace YandexConverter
{
    public class GrpcAnswer
    {
        [JsonProperty("chunks")]
        public List<Chunk> Chunks { get; set; }
    }
    public class Chunk
    {
        [JsonProperty("alternatives")]
        public List<Alternative> Alternatives { get; set; }
        [JsonProperty("final", NullValueHandling = NullValueHandling.Ignore)]
        public bool Final { get; set; }
        [JsonProperty("endOfUtterance", NullValueHandling = NullValueHandling.Ignore)]
        public bool EndOfUtterance { get; set; }
    }
    public class Alternative
    {
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("confidence")]
        public int Confidence { get; set; }
    }
    

    
}