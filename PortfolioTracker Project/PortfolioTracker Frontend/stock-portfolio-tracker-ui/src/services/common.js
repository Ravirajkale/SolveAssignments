import axios from 'axios';

const API_BASE_URL = 'http://localhost:5210/api/auth'; // Adjust to match your backend API base URL

const register = async (firstName, lastName, email, password) => {
    try {
        const response = await axios.post(`${API_BASE_URL}/register`, {
            firstName,
            lastName,
            email,
            password,
        });
        return response.data;
    } catch (error) {
        console.error("Registration error:", error);
        throw error; // Re-throw to handle in components
    }
};

const login = async (email, password) => {
    try {
        const response = await axios.post(`${API_BASE_URL}/login`, {
            email,
            password,
        });
        return response.data;
    } catch (error) {
        console.error("Login error:", error);
        throw error; // Re-throw to handle in components
    }
};

export { register, login };