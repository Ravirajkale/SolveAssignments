using System.Text.Json.Serialization;

namespace PortfolioTrackerApi.DTOS
{
    public class GlobalQuoteResponse
    {
        [JsonPropertyName("Global Quote")]
        public GlobalQuote GlobalQuote { get; set; }
    }
}
