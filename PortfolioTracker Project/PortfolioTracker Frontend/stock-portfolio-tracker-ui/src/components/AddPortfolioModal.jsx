import React, { useState, useRef, useEffect } from 'react';
import PropTypes from 'prop-types';
import './AddPortfolioModal.css'; // Create this CSS file

function AddPortfolioModal({ isOpen, onClose, onAdd }) {
    const [portfolioName, setPortfolioName] = useState('');
    const modalRef = useRef(null);

    useEffect(() => {
        function handleClickOutside(event) {
            if (modalRef.current && !modalRef.current.contains(event.target)) {
                onClose(); // Close the modal when clicking outside
            }
        }

        // Bind the event listener
        document.addEventListener("mousedown", handleClickOutside);
        return () => {
            // Unbind the event listener on clean up
            document.removeEventListener("mousedown", handleClickOutside);
        };
    }, [modalRef, onClose]);

    const handleInputChange = (e) => {
        setPortfolioName(e.target.value);
    };

    const handleSubmit = (e) => {
        e.preventDefault();
        if (portfolioName.trim() !== '') {
            onAdd(portfolioName);
            onClose();
        }
    };

    if (!isOpen) return null; // Don't render if not open

    return (
        <div className="modal-overlay">
            <div className="modal-content" ref={modalRef}>
                <h2>Add New Portfolio</h2>
                <form onSubmit={handleSubmit}>
                    <input
                        type="text"
                        placeholder="Portfolio Name"
                        value={portfolioName}
                        onChange={handleInputChange}
                        className="modal-input"
                    />
                    <div className="modal-buttons">
                        <button type="submit" className="modal-add-button">Add</button>
                        <button type="button" className="modal-cancel-button" onClick={onClose}>Cancel</button>
                    </div>
                </form>
            </div>
        </div>
    );
}

AddPortfolioModal.propTypes = {
    isOpen: PropTypes.bool.isRequired,
    onClose: PropTypes.func.isRequired,
    onAdd: PropTypes.func.isRequired,
};

export default AddPortfolioModal;