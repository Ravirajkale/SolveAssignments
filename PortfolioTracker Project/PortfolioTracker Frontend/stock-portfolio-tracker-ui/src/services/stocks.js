import axios from "axios";

const API_BASE_URL = "http://localhost:5210/api/stocks"; // Update as per your backend

// Fetch all stocks from the backend (without pagination)
export async function getStocks(token) {
    try {
        const response = await axios.get(`${API_BASE_URL}/available`, {
            headers: {
                Authorization: `Bearer ${token}`,
            },
        });
        return response.data; // Return the entire list of stocks
    } catch (error) {
        console.error("Error fetching stocks:", error);
        throw error; // Re-throw the error for the component to handle
    }
}

// Search for a stock by ticker (calls backend method that integrates with Alpha Vantage)
export async function searchStock(ticker, token) {
    try {
        const response = await axios.get(`${API_BASE_URL}/search/${ticker}`, {
            headers: {
                Authorization: `Bearer ${token}`,
            },
        });
        return response.data; // Return the stock details from Alpha Vantage API
    } catch (error) {
        console.error(`Error searching stock ${ticker}:`, error);
        throw error; // Re-throw the error for the component to handle
    }
}

export async function searchStockalike(query, token) {
    try {
        const response = await axios.get(`${API_BASE_URL}/search?query=${encodeURIComponent(query)}`, {
            headers: {
                Authorization: `Bearer ${token}`,
            },
        });
        return response.data; // List of matching stocks with price and last updated
    } catch (error) {
        console.error(`Error searching stock "${query}":`, error);
        throw error;
    }
}

// Add a new stock to the portfolio
export async function addStockToPortfolio(body, token) {
    try {
        const response = await axios.post(`${API_BASE_URL}/add`, body, {
            headers: {
                Authorization: `Bearer ${token}`,
                "Content-Type": "application/json",
            },
        });
        return response.data; // Return the entire response data
    } catch (error) {
        console.error("Error adding stock:", error);
        throw error; // Re-throw the error for the component to handle
    }
}

export async function getPortfolioStocks(portfolioId, token) {
    try {
        const response = await axios.get(`${API_BASE_URL}/${portfolioId}/stocks`, {
            headers: {
                Authorization: `Bearer ${token}`,
            },
        });
        return response.data; // Return the response data
    } catch (error) {
        console.error("Error fetching portfolio stocks:", error);
        throw error; // Re-throw the error for handling in components
    }
}