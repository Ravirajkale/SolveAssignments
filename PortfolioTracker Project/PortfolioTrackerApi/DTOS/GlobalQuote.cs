using System.Text.Json.Serialization;

namespace PortfolioTrackerApi.DTOS
{
    public class GlobalQuote
    {
        [JsonPropertyName("01. symbol")]
        public string Symbol { get; set; }

        [JsonPropertyName("05. price")]
        public string Price { get; set; }
    }
}

