namespace PortfolioTrackerApi.DTOS
{
    public class PortfolioResponseDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int StocksCount { get; set; }
        public decimal PurchaseValue { get; set; }
        public decimal CurrentValue { get; set; }
        public int PortfoliosWithNoStocks { get; set; }
    }
}
