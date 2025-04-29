namespace PortfolioTrackerApi.DTOS
{
    public class HistoricalChartPointDto
    {
        public DateTime Date { get; set; }
        public decimal ClosingPrice { get; set; }
    }
}
