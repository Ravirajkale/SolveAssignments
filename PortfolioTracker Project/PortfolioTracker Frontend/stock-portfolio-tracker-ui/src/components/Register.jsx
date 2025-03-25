import React, { useState } from 'react';
import { useNavigate,Link } from 'react-router-dom';
import { register } from '../services/common';
import { toast } from 'react-toastify';
import './Register.css';

function Register() {
    const [firstName, setFirstName] = useState('');
    const [lastName, setLastName] = useState('');
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const navigate = useNavigate();

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');

        try {
            const data = await register(firstName, lastName, email, password);

            if (data && data.message) { // Expecting a 'message' property
                toast.success(data.message);
                navigate('/login'); // Redirect to login page
            } else {
                setError('Registration failed. Please try again.');
                toast.error('Registration failed. Please try again.');
            }
        } catch (err) {
            console.error("Registration error:", err);
            setError('Registration failed. Please try again.');
            toast.error('Registration failed. Please try again.');
        }
    };

    return (
        <div className="register-container">
            <div className="register-card">
                <h2>Register</h2>
                {error && <p className="error-message">{error}</p>}
                <form onSubmit={handleSubmit}>
                     <input
                        type="text"
                        placeholder="First Name"
                        value={firstName}
                        onChange={(e) => setFirstName(e.target.value)}
                        className="register-input"
                    />
                    <input
                        type="text"
                        placeholder="Last Name"
                        value={lastName}
                        onChange={(e) => setLastName(e.target.value)}
                        className="register-input"
                    />
                    <input
                        type="text"
                        placeholder="Email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        className="register-input"
                    />
                    <input
                        type="password"
                        placeholder="Password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        className="register-input"
                    />
                    <button type="submit" className="register-button">Register</button>
                </form>
                <p>
                    Already have an account? <Link to="/login">Login here</Link>
                </p>
            </div>
        </div>
    );
}

export default Register;