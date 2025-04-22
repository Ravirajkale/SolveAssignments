import React, { useState } from "react";
import "./PortfolioStockCard.css";

const PortfolioStockCard = React.memo(({ stock, onUpdate, onDelete }) => {
    const [editing, setEditing] = useState(false);
    const [quantity, setQuantity] = useState(stock.quantity);

    const profitLoss = stock.totalCurrentValue - stock.totalPurchasedValue;
    const isProfit = profitLoss >= 0;

    const handleUpdateClick = () => {
        if (editing) {
            onUpdate(stock.stockId, quantity); // Save action
        }
        setEditing(!editing); // Toggle edit mode
    };

    return (
        <div className="stock-card">
            <div>
                <h2 className="stock-title">
                    {stock.company} ({stock.ticker})
                </h2>
                <p className="stock-currentPrice">Current Price: ₹{stock.currentPrice}</p>
                <p className="stock-price">Purchase Price: ₹{stock.purchasePrice}</p>
                <p className="stock-quantity">
                    Quantity:{" "}
                    {editing ? (
                        <input
                            type="number"
                            value={quantity}
                            onChange={(e) => setQuantity(parseInt(e.target.value))}
                            min={1}
                            className="quantity-input"
                        />
                    ) : (
                        stock.quantity
                    )}
                </p>
                <p className="stock-purchaseValue">Purchased Value: ₹{stock.totalPurchasedValue}</p>
                <p className="stock-currentValue">Current Value: ₹{stock.totalCurrentValue}</p>
                <p className={`stock-currentValue ${isProfit ? "profit" : "loss"}`}>
                    {isProfit
                        ? `+₹${profitLoss.toFixed(2)}`
                        : `-₹${Math.abs(profitLoss).toFixed(2)}`}
                </p>

                <div className="stock-actions">
                    <button className="update-btn" onClick={handleUpdateClick}>
                        {editing ? "Save" : "Update"}
                    </button>
                    <button className="delete-btn" onClick={() => onDelete(stock.stockId)}>
                        Delete
                    </button>
                </div>
            </div>
        </div>
    );
});

export default PortfolioStockCard;
