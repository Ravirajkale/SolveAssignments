import axios from 'axios';

const API_BASE_URL = 'http://localhost:5210/api/portfolios'; 

export async function getPortfolios(pageNumber, pageSize, token) {
   // console.log("inside get portfolios");
    try {
        const response = await axios.get(`${API_BASE_URL}?pageNumber=${pageNumber}&pageSize=${pageSize}`, {
            headers: {
                Authorization: `Bearer ${token}`,
            },
        });
        return response.data; // Return the entire response data
    } catch (error) {
        console.error("Error fetching portfolios:", error);
        throw error; // Re-throw the error for the component to handle
    }
}

export async function createPortfolio(body, token) {
    try {
        const response = await axios.post(`${API_BASE_URL}`, body, {
            headers: {
                Authorization: `Bearer ${token}`,
            },
        });
        return response.data; // Return the entire response data
    } catch (error) {
        console.error("Error creating portfolio:", error);
        throw error; // Re-throw the error for the component to handle
    }
}

export const updateStockQuantity = async (stockId, newQuantity,token) => {
    try {
      const response = await axios.put(`${API_BASE_URL}/quantity`, {
        stockId,
        newQuantity,
      }, {
        headers: {
            Authorization: `Bearer ${token}`,
        },
    });
      return response.data;
    } catch (error) {
      console.error("Error updating stock quantity:", error);
      throw error;
    }
  };

  export const deleteStock = async (stockId,token) => {
    try {
      const response = await axios.delete(`${API_BASE_URL}/${stockId}`, {
        headers: {
            Authorization: `Bearer ${token}`,
        },
    });
      return response.data;
    } catch (error) {
      console.error("Error deleting stock:", error);
      throw error;
    }
  };