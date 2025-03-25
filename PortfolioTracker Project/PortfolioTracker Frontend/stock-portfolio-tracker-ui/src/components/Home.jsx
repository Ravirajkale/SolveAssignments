import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../AuthContext';
import { getPortfolios } from '../services/portfolios';
import './Home.css';
import PortfolioCard from './PortfolioCard'; // Import the Card component

function Home() {
    const [portfolios, setPortfolios] = useState([]);
    const [currentPage, setCurrentPage] = useState(1);
    const [pageSize] = useState(10); // Display 10 portfolios per page
    const [totalCount, setTotalCount] = useState(0);
    const { token } = useAuth();

    useEffect(() => {
        const fetchPortfolios = async () => {
            try {
                
                const data = await getPortfolios(currentPage, pageSize, token); // Use the service
               // console.log(data.totalCount);
                setPortfolios(data && data.portfolios ? data.portfolios : []); // Ensure portfolios is an array
                setTotalCount(data && data.totalCount ? data.totalCount : 0);
            } catch (error) {
                console.error("Error fetching portfolios:", error);
            }
        };

        fetchPortfolios();
    }, [currentPage, pageSize, token]);

    const totalPages = Math.ceil(totalCount / pageSize);

    const handlePageChange = (newPage) => {
        setCurrentPage(newPage);
    };

    return (
        <div className="home-container">
            <h2>My Portfolios</h2>
            <Link to="/AddPortfolio" className="add-portfolio-link">
                <button className="createPortfolioButton">Create Portfolio</button>
            </Link>
            <div className="portfolio-grid">
                {totalCount === 0 ? (
                    <p>You don't have any portfolios yet. Add one to get started!</p>
                ) : (
                    portfolios.map(portfolio => (
                        <PortfolioCard key={portfolio.id} portfolio={portfolio} />//Use the portfolio card!
                    ))
                )}
            </div>
            {totalCount > 0 && (
                <div className="pagination-controls">
                    <button
                        onClick={() => handlePageChange(currentPage - 1)}
                        disabled={currentPage === 1}
                        className="pagination-button"
                    >
                        Previous
                    </button>
                    {Array.from({ length: totalPages }, (_, i) => i + 1).map(page => (
                        <button
                            key={page}
                            onClick={() => handlePageChange(page)}
                            disabled={currentPage === page}
                            className="pagination-button"
                        >
                            {page}
                        </button>
                    ))}
                    <button
                        onClick={() => handlePageChange(currentPage + 1)}
                        disabled={currentPage === totalPages}
                        className="pagination-button"
                    >
                        Next
                    </button>
                </div>
            )}
        </div>
    );
}

export default Home;