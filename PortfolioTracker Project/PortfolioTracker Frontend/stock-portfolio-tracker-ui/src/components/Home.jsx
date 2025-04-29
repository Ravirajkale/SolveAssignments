import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../AuthContext';
import { getPortfolios,createPortfolio } from '../services/portfolios';
import './Home.css';
import PortfolioCard from './PortfolioCard'; // Import the Card component
import AddPortfolioModal from './AddPortfolioModal';

function Home() {
    const [portfolios, setPortfolios] = useState([]);
    const [currentPage, setCurrentPage] = useState(1);
    const [pageSize] = useState(10); // Display 10 portfolios per page
    const [totalCount, setTotalCount] = useState(0);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const { token,logout } = useAuth();

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

    const handleAddPortfolioClick = () => {
        setIsModalOpen(true); // Open the modal
    };

    const handleCloseModal = () => {
        setIsModalOpen(false); // Close the modal
        //Set name back.
    };

    const handleAddPortfolio = async (portfolioName) => {
        try {
              //setMessage("Adding!");
            const data = await createPortfolio( { name: portfolioName }, token); //Call to backend function.
            console.log(data);
            if (data.name==portfolioName && totalCount<pageSize)
            {
                 setTotalCount(prevCount => prevCount + 1);
                 setPortfolios(prevPortfolios => [...prevPortfolios, { id: data.id, name: data.name}]);
                   toast.success("Added Succesfully.");
                   //add to current code

            }
            else if(data.name==portfolioName && totalCount>pageSize){
                toast.success("Added Succesfully");
            }
            else{
                throw console.error("data failed or not");
            }
               // setMessage("");
        } catch (error) {
            console.log("Error " + error);
        }
    };

    return (
        <div className="home-container">
             <div className="home-header">
                <h2>My Portfolios</h2>
                <button className="logoutButton" onClick={logout}>
                    Logout
                </button>
            </div>
            <button className = "createPortfolioButton" onClick={handleAddPortfolioClick}>Create Portfolio</button>
            <button className="statisticsButton">
                <Link to="/statistics" style={{ textDecoration: 'none', color: 'inherit' }}>
                     Show Statistics
                </Link>
            </button>
            <AddPortfolioModal
                isOpen={isModalOpen}
                onClose={handleCloseModal}
                onAdd={handleAddPortfolio}
            />
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