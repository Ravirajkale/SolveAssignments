// Function to fetch the stock data from the portfolio table
function fetchPortfolioTableData() {
    const rows = document.querySelectorAll('table tbody tr');
    const stocks = [];

    rows.forEach(row => {
        const columns = row.querySelectorAll('td');
        if (columns.length >= 3) {
            const ticker = columns[0]?.innerText.trim();
            const last_price = columns[1]?.innerText.trim();
            const change = columns[2]?.innerText.trim();
            const currency=columns[4]?.innerText.trim();

            stocks.push({ ticker, last_price, change,currency });
        }
    });

    console.clear();
    console.table(stocks);
    console.log(stocks);
    return stocks;
}

// Fetch the portfolio data and send it to the server for storage
function sendStockDataToServer(stocks) {

    const token='eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjA0MzRiOWEwLTNmN2UtNDRiNC1hZTM4LWY5ZTI0NzJmNjU1OCIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL2VtYWlsYWRkcmVzcyI6IlJhdmkxMjNALmNvbSIsImV4cCI6MTc0NTg0MTg3MSwiaXNzIjoiQVBJIiwiYXVkIjoiQ2xpZW50In0.45KSNa1oXeAhdO_Fza60geohbYPGHNi15lYIopeAJr0';
    fetch('http://localhost:5210/api/stocks/save', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`,
        },
        body: JSON.stringify(stocks),
    })
    .then(response => response.json())
    .then(data => {
        console.log('Stock data successfully saved:', data);
    })
    .catch(error => {
        console.error('Error saving stock data:', error);
    });
}

// Regularly fetch and send the stock data every minute (60000 ms)
setInterval(() => {
    const portfolioStocks = fetchPortfolioTableData();
    sendStockDataToServer(portfolioStocks);
}, 60000); // Every minute