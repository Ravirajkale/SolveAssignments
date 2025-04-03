import React, { useState, useEffect } from "react";
import { useParams } from "react-router-dom"; 
import { useAuth } from '../AuthContext';
import { getStocks, searchStock, addStockToPortfolio } from "../services/stocks";
import StockCard from "../components/StockCard";
import "./AddStocks.css"; // Import CSS

function AddStocks () {
    const { portfolioId } = useParams(); 
    const [stocks, setStocks] = useState([]); 
    const [filteredStocks, setFilteredStocks] = useState([]); 
    const [searchTerm, setSearchTerm] = useState(""); 
    const [selectedStock, setSelectedStock] = useState(null); 
    const [quantity, setQuantity] = useState(1); 
    const { token } = useAuth();

    useEffect(() => {
        const fetchStocks = async () => {
            try {
                const data = await getStocks(token);
                setStocks(data);
                setFilteredStocks(data);
            } catch (error) {
                console.error("Error fetching stocks:", error);
            }
        };
        fetchStocks();
    }, [token]);

    const handleSearch = (e) => {
        const value = e.target.value;
        setSearchTerm(value);

        if (value.trim() === "") {
            setFilteredStocks(stocks); 
        } else {
            const filtered = stocks.filter((stock) =>
                stock.company.toLowerCase().includes(value.toLowerCase()) ||
                stock.ticker.toLowerCase().includes(value.toLowerCase())
            );
            setFilteredStocks(filtered);
        }
    };

    const handleSearchBackend = async () => {
        if (!searchTerm) return;
        try {
            const stock = await searchStock(searchTerm, token);
            setFilteredStocks(stock ? [stock] : []);
        } catch (error) {
            console.error("Error fetching stock from API:", error);
        }
    };

    const handleAddStock = async () => {
        if (!selectedStock) return;
        try {
            await addStockToPortfolio(
                { portfolioId,
                    name: selectedStock.company, // Assuming company is the stock name
                    ticker: selectedStock.ticker,
                    quantity,
                    purchasePrice: selectedStock.currentPrice },
                token
            );
            alert("Stock added successfully!");
            setSelectedStock(null); 
            setQuantity(1);
        } catch (error) {
            console.error("Error adding stock:", error);
        }
    };

    return (
        <div className="add-stocks-container">
            <h1 className="add-stocks-title">Add Stocks</h1>

            {/* Search Input */}
            <div className="search-container">
                <input
                    type="text"
                    className="search-input"
                    placeholder="Search stock by name or symbol..."
                    value={searchTerm}
                    onChange={handleSearch}
                />
                <button className="search-button" onClick={handleSearchBackend}>
                    Search
                </button>
            </div>

            {/* Scrollable Stock List */}
            <div className="stocks-list">
                {filteredStocks.length > 0 ? (
                    filteredStocks.map((stock) => (
                        <StockCard key={stock.ticker} stock={stock} onAdd={setSelectedStock} />
                    ))
                ) : (
                    <p className="no-stocks">No matching stocks found. Try searching.</p>
                )}
            </div>

            {/* Add Stock Modal */}
            {selectedStock && (
                <div className="modal-overlay">
                    <div className="modal">
                        <h2 className="modal-title">{selectedStock.company} ({selectedStock.ticker})</h2>
                        <p className="modal-price">Price: ₹{selectedStock.currentPrice}</p>

                        <label className="modal-label">Quantity:</label>
                        <input
                            type="number"
                            min="1"
                            className="modal-input"
                            value={quantity}
                            onChange={(e) => setQuantity(e.target.value)}
                        />

                        <div className="modal-buttons">
                            <button className="add-button" onClick={handleAddStock}>
                                Add to Portfolio
                            </button>
                            <button className="cancel-button" onClick={() => setSelectedStock(null)}>
                                Cancel
                            </button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
};

export default AddStocks;