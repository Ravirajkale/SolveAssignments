import React, { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { getPortfolioStocks } from "../services/stocks";
import { useAuth } from "../AuthContext";
import { useLocation } from "react-router-dom";
import { subscribeToWebSocket, unsubscribeFromWebSocket } from "../services/WebSocketService"; // Import WebSocket service
import './PortfolioStocks.css';
const PortfolioStocks = () => {
    const { portfolioId } = useParams();
    const navigate = useNavigate();
    const [stocks, setStocks] = useState([]);
    const [filteredStocks, setFilteredStocks] = useState([]);
    const [searchQuery, setSearchQuery] = useState("");
    const [loading, setLoading] = useState(true);
    const { token } = useAuth();
    const [error, setError] = useState(null);
    const location = useLocation();
    const portfolioName = location.state?.name || "Portfolio";

    const fetchPortfolioStocks = async () => {
        try {
            setLoading(true);
            const data = await getPortfolioStocks(portfolioId, token);
            setStocks(data);
            setFilteredStocks(data);
        } catch (err) {
            setError("Error fetching portfolio stocks.");
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchPortfolioStocks();

        // Subscribe to WebSocket updates
        subscribeToWebSocket(fetchPortfolioStocks);

        return () => {
            unsubscribeFromWebSocket(fetchPortfolioStocks);
        };
    }, [portfolioId, token]);

    // Handle search input
    const handleSearchChange = (e) => {
        const query = e.target.value.toUpperCase();
        setSearchQuery(query);

        if (query === "") {
            setFilteredStocks(stocks);
        } else {
            setFilteredStocks(stocks.filter((stock) => stock.ticker.includes(query)));
        }
    };

    return (
        <div className="portfolio-container">
            <h1 className="portfolio-title">{portfolioName} Stocks</h1>

            <div className="button-container">
                <button className="add-stock-btn" onClick={() => navigate(`/portfolio/${portfolioId}/addstocks`)}>
                    Add Stocks
                </button>
                <button className="statistics-btn" onClick={() => alert("Show statistics page coming soon!")}>
                    Show Statistics
                </button>
            </div>

            {/* Search Bar */}
            <input
                type="text"
                placeholder="Search by stock symbol..."
                value={searchQuery}
                onChange={handleSearchChange}
                className="search-bar"
            />

            {loading ? (
                <p className="loading-text">Loading stocks...</p>
            ) : error ? (
                <p className="error-text">{error}</p>
            ) : (
                <div className="stocks-list">
                    {filteredStocks.length > 0 ? (
                        filteredStocks.map((stock, index) => {
                            const profitLoss = stock.totalCurrentValue - stock.totalPurchasedValue;
                            const isProfit = profitLoss >= 0;
                            return (
                                <div key={`${stock.ticker}-${index}`} className="stock-card">
                                    <div>
                                        <h2 className="stock-title">{stock.company} ({stock.ticker})</h2>
                                        <p className="stock-currentPrice">Current Price: ₹{stock.currentPrice}</p>
                                        <p className="stock-price">Purchase Price: ₹{stock.purchasePrice}</p>
                                        <p className="stock-quantity">Quantity: {stock.quantity}</p>
                                        <p className="stock-purchaseValue">Purchased Value: ₹{stock.totalPurchasedValue}</p>
                                        <p className="stock-currentValue">Current Value: ₹{stock.totalCurrentValue}</p>
                                        <p 
                                            className={`stock-currentValue ${isProfit ? "profit" : "loss"}`}
                                        >
                                            {isProfit 
                                                ? `+₹${profitLoss.toFixed(2)}` 
                                                : `-₹${Math.abs(profitLoss).toFixed(2)}`}
                                        </p>
                                    </div>
                                </div>
                            );
                        })
                    ) : (
                        <p className="no-stocks-text">No matching stocks found.</p>
                    )}
                </div>
            )}
        </div>
    );
};

export default PortfolioStocks;
