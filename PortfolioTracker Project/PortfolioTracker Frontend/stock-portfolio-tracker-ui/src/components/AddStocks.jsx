import React, { useState, useEffect } from "react";
import { useParams } from "react-router-dom"; 
import { useAuth } from '../AuthContext';
import { debounce, isString } from "lodash";
import { getStocks, searchStock, addStockToPortfolio, searchStockalike } from "../services/stocks";
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
        debouncedSearch(value);
    };

    const handleSearchApi=async(value)=>{
        if (!value) return;
        try {
            const stock = await searchStock(value, token);
            if (stock) {
                setFilteredStocks([stock]);
            } else {
                alert("Stock not found or no data available.");
                setFilteredStocks([]);
            }
        } catch (error) {
            console.error("Error fetching stock from API:", error);
            alert("Something went wrong while searching the stock.");
        }
    };
   // 500ms delay
    const handleSearchBackend = async (value) => {
      if(!value || typeof value !== "string" || !value.trim() ){
        setFilteredStocks([])
      }
        if (!value.trim()) {
            setFilteredStocks(stocks); // or reset to initial stocks if needed
            return;
        }
    
        try {
            const result = await searchStockalike(value, token);
            setFilteredStocks(result || []);
        } catch (error) {
            console.error("Error fetching stock from backend:", error);
        }
    };
    const debouncedSearch = debounce(handleSearchBackend, 200);
    const handleQuantityChange = (e) => {
        const value = e.target.value;
        const numValue = Number(value);
    
        // Allow only numbers between 1 and 1,000,000
        if (numValue >= 1 && numValue <= 1000000) {
            setQuantity(numValue);
        } else {
            setQuantity(""); // Reset if out of bounds
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
                <button className="search-button" onClick={() => handleSearchApi(searchTerm)}>
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
                            onChange={handleQuantityChange}
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