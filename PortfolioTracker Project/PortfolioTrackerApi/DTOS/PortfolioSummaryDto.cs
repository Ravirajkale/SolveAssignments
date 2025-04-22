namespace PortfolioTrackerApi.DTOS
{
    public class PortfolioSummaryDto
    {
        public decimal TotalInvested { get; set; }
        public decimal TotalCurrentValue { get; set; }
        public decimal OverallProfitLoss { get; set; }
        public int PortfoliosInProfit { get; set; }
        public int PortfoliosInLoss { get; set; }

        public int PortfoliosWithNoStocks { get; set; }
    }
}
