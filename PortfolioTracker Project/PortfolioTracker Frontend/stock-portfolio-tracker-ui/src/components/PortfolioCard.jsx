import React from 'react';
import { Link } from 'react-router-dom';
import PropTypes from 'prop-types';
import './PortfolioCard.css';
function PortfolioCard({ portfolio }) {
    return (
        <Link
            to={`/portfolio/${portfolio.id}/stocks`}
            state={{ name: portfolio.name }}
            className="portfolio-card"
            style={{ textDecoration: 'none' }} // Remove underline
        >
            <div className="card-content">
                <h3>{portfolio.name}</h3>
                <p>Number of Stocks: {portfolio.stocksCount}</p> {/* Display the StockCount */}
            </div>
        </Link>
    );
}

PortfolioCard.propTypes = {
    portfolio: PropTypes.shape({
        id: PropTypes.number.isRequired,
        name: PropTypes.string.isRequired,
        StocksCount: PropTypes.number.isRequired,
    }).isRequired,
};

export default PortfolioCard;