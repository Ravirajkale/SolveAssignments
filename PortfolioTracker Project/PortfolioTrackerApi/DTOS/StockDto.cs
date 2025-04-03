namespace PortfolioTrackerApi.DTOS
{
    public class StockDto
    {
        public string Ticker { get; set; }
        public string Company { get; set; }
        public decimal CurrentPrice { get; set; }

        public decimal PurchasePrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalCurrentValue { get; set; }
        public decimal TotalPurchasedValue { get; set; } // Purchased price * Quantity
    }
}
