namespace PortfolioTrackerApi.Controllers
{
    public class ScrapperDto
    {
        public string ticker { get; set; }
        public string last_price { get; set; }
        public string currency { get; set; }
        public string change { get; set; }
    }
}