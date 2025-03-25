import React, { useState } from 'react';
import { useNavigate,Link } from 'react-router-dom';
import { useAuth } from '../AuthContext';
import { login } from '../services/common'; // Import the login service
import { toast } from 'react-toastify';
import './Login.css';

function Login() {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const navigate = useNavigate();
    const { login: authLogin } = useAuth(); // Renamed to avoid naming conflict

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');

        try {
            const data = await login(email, password); // Use the login service

            if (data && data.token) {
                localStorage.setItem('token', data.token);
                console.log(data.token);
                 await authLogin(data.token);
                toast.success(`Hey, ${data.firstName} Welcome aboard!`);
                navigate('/home');
            } else {
                setError('Login failed. Please check your credentials.');
                toast.error("Invalid Credentials :<");
            }
        } catch (err) {
            console.error("Login error:", err);
            setError('Login failed. Please check your credentials.');
            toast.error("Invalid Credentials :<");
        }
    };

    return (
        <div className="login-container">
        <div className="login-card">
            <h2>Login</h2>
            {error && <p className="error-message">{error}</p>}
            <form onSubmit={handleSubmit}>
                <input
                    type="text"
                    placeholder="Email"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    className="login-input"
                />
                <input
                    type="password"
                    placeholder="Password"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    className="login-input"
                />
                <button type="submit" className="login-button">Login</button>
            </form>
            <p>
                Don't have an account? <Link to="/register">Register here</Link>
            </p>
        </div>
    </div>
    );
}

export default Login;