namespace PortfolioTrackerApi.DTOS
{
    public class StockAddDto
    {
        public int PortfolioId { get; set; }
        public string Name { get; set; }
        public string Ticker { get; set; }
        public int Quantity { get; set; }
        public decimal PurchasePrice { get; set; }
    }
}
