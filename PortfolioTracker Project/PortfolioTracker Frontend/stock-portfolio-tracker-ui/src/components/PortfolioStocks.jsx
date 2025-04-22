import React, { useState, useEffect } from "react";
import { useParams, useNavigate, useLocation } from "react-router-dom";
import { getPortfolioStocks } from "../services/stocks";
import { updateStockQuantity, deleteStock } from "../services/portfolios";
import { useAuth } from "../AuthContext";
import { subscribeToWebSocket, unsubscribeFromWebSocket } from "../services/WebSocketService";
import './PortfolioStocks.css';
import PortfolioStockCard from "./PortfolioStockCard";

const PortfolioStocks = () => {
    const { portfolioId } = useParams();
    const navigate = useNavigate();
    const [stocks, setStocks] = useState([]);
    const [originalStocks, setOriginalStocks] = useState([]); 
    const [filteredStocks, setFilteredStocks] = useState([]);
    const [searchQuery, setSearchQuery] = useState("");
    const [loading, setLoading] = useState(true);
    const { token } = useAuth();
    const [error, setError] = useState(null);
    const location = useLocation();
    const portfolioName = location.state?.name || "Portfolio";

    const fetchPortfolioStocks = async () => {
        try {
            const data = await getPortfolioStocks(portfolioId, token);
            setOriginalStocks(data); //  Store full list for filtering/reset
            console.log("Fetched portfolio data:", data);
            setStocks((prevStocks) => {
                return prevStocks.map((prevStock) => {
                    const updated = data.find(s => s.ticker === prevStock.ticker);
                    return updated
                        ? { ...prevStock, currentPrice: updated.currentPrice, totalCurrentValue: updated.totalCurrentValue }
                        : prevStock;
                });
            });

            setFilteredStocks(data); // Display initially
        } catch (err) {
            setError("Error fetching portfolio stocks.");
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchPortfolioStocks();
        subscribeToWebSocket(fetchPortfolioStocks);
        return () => unsubscribeFromWebSocket(fetchPortfolioStocks);
    }, [portfolioId, token]);

    const handleUpdateStock = async (stockId, newQuantity) => {
        try {
            await updateStockQuantity(stockId, newQuantity,token);
            fetchPortfolioStocks(); // Refresh list
        } catch (err) {
            console.error("Update failed:", err);
        }
    };
    
    const handleDeleteStock = async (stockId) => {
        try {
            await deleteStock(stockId,token);
            fetchPortfolioStocks(); // Refresh list
        } catch (err) {
            console.error("Delete failed:", err);
        }
    };

    const handleSearchChange = (e) => {
        const query = e.target.value.toUpperCase();
        setSearchQuery(query);

        if (!query.trim()) {
            setFilteredStocks(originalStocks); // ✅ Reset list after clearing search
        } else {
            setFilteredStocks(
                originalStocks.filter((stock) => stock.ticker.includes(query))
            );
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
                <div className="stocks-list-portfolio">
                    {filteredStocks.length > 0 ? (
                        filteredStocks.map((stock, index) => (
                            <PortfolioStockCard key={`${stock.ticker}-${index}`} stock={stock}  onUpdate={handleUpdateStock}
                            onDelete={handleDeleteStock} />
                        ))
                    ) : (
                        <p className="no-stocks-text">No matching stocks found.</p>
                    )}
                </div>
            )}
        </div>
    );
};

export default PortfolioStocks;
