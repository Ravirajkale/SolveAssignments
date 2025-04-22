import React from 'react';
import { Link } from 'react-router-dom';
import PropTypes from 'prop-types';
import './PortfolioCard.css';

function PortfolioCard({ portfolio }) {
    const {
        id,name,stocksCount,purchaseValue = 0,currentValue = 0,} = portfolio;

        let valueStyle = { fontWeight: 'bold' };
        let statusText = '';
    
        if (currentValue > purchaseValue) {
            valueStyle.color = 'green';
            statusText = '↑ Profit';
        } else if (currentValue < purchaseValue) {
            valueStyle.color = 'red';
            statusText = '↓ Loss';
        } else {
            valueStyle.color = 'gray';
            statusText = '→ No Gain or Loss';
        }

    return (
        <Link
            to={`/portfolio/${id}/stocks`}
            state={{ name }}
            className="portfolio-card"
            style={{ textDecoration: 'none' }}
        >
            <div className="card-content">
                <h3>{name}</h3>
                <p>Number of Stocks: {stocksCount}</p>
                <p>Purchase Value: ₹{purchaseValue.toFixed(2)}</p>
                <p style={valueStyle}>
                    Current Value: ₹{currentValue.toFixed(2)}
                </p>
                <p style={valueStyle}>
                    {statusText}
                </p>
            </div>
        </Link>
    );
}

PortfolioCard.propTypes = {
    portfolio: PropTypes.shape({
        id: PropTypes.number.isRequired,
        name: PropTypes.string.isRequired,
        stocksCount: PropTypes.number.isRequired,
        purchaseValue: PropTypes.number,
        currentValue: PropTypes.number
    }).isRequired,
};

export default PortfolioCard;