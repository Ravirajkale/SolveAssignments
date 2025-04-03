import React from "react";
import "./StockCard.css"; // Import CSS file

const StockCard = ({ stock, onAdd }) => {
    return (
        <div className="stock-card">
            <div className="stock-info">
                <h2 className="stock-title">{stock.company} ({stock.ticker})</h2>
                <p className="stock-price">Price: ₹{stock.currentPrice}</p>
                <p className="stock-updated">Last Updated: {new Date(stock.lastUpdated).toLocaleString()}</p>
            </div>
            <button className="stock-add-button" onClick={() => onAdd(stock)}>
                Add
            </button>
        </div>
    );
};

export default StockCard;
