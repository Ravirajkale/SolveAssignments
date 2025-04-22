import axios from "axios";

const API_BASE_URL = "http://localhost:5210/api/statistics"; 

export const getPortfolioSummary = async (token) => {
    try {
        const response = await axios.get(`${API_BASE_URL}/portfolio-summary`, {
            headers: {
                Authorization: `Bearer ${token}`
            }
        });
        return response.data;
    } catch (error) {
        console.error("Error fetching portfolio summary:", error);
        throw error;
    }
};

export const getInitialChartData = async (date, token) => {
    const res = await axios.get(`${API_BASE_URL}/default-historical-charts/${date}`, {
        headers: { Authorization: `Bearer ${token}` },
    });
    return res.data;
};

export const getHistoricalChartData = async (ticker, token, date) => {
    const res = await axios.get(
        `${API_BASE_URL}/historical-chart/${ticker}?date=${date}`,
        {
            headers: { Authorization: `Bearer ${token}` },
        }
    );
    return res.data;
};

export const getAvailableDates = async (token) => {
    const res = await axios.get(`${API_BASE_URL}/available-dates`, {
        headers: { Authorization: `Bearer ${token}` },
    });
    return res.data;
};