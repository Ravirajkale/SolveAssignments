import React from "react";
import "./PortfolioSummaryCard.css";

const PortfolioSummaryCard = React.memo(({ summary }) => {
    const isProfit = summary.totalCurrentValue >= summary.totalInvested;
    const profitLossStyle = {
        color: isProfit ? "green" : "red",
        fontWeight: "bold"
    };

    return (
        <div className="summary-card">
            <h3>Overall Portfolio Summary</h3>
            <div className="summary-grid">
                <div className="summary-item">
                    <span>Total Invested:</span>
                    <span  style={{ color: "#007bff", fontWeight: "bold" }}>₹{summary.totalInvested.toFixed(2)}</span>
                </div>
                <div className="summary-item">
                    <span>Total Current Value:</span>
                    <span style={{ color: "#007bff", fontWeight: "bold" }}>₹{summary.totalCurrentValue.toFixed(2)}</span>
                </div>
                <div className="summary-item">
                    <span>Profit / Loss:</span>
                    <span style={profitLossStyle}>₹{summary.overallProfitLoss.toFixed(2)}</span>
                </div>
                <div className="summary-item">
                    <span>Portfolios in Profit:</span>
                    <span  style={{ color: "green", fontWeight: "bold" }}>{summary.portfoliosInProfit}</span>
                </div>
                <div className="summary-item">
                    <span>Portfolios in Loss:</span>
                    <span style={{ color: "red", fontWeight: "bold" }}>{summary.portfoliosInLoss}</span>
                </div>
                <div className="summary-item">
                    <span>Portfolios with No Stocks:</span>
                    <span>{summary.portfoliosWithNoStocks}</span>
                </div>
            </div>
        </div>
    );
});

export default PortfolioSummaryCard;