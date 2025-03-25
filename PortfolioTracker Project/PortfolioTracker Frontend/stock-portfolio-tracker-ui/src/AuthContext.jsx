import React, { createContext, useState, useContext, useEffect } from 'react';

const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
  const [jwtToken, setJwtToken] = useState(() => localStorage.getItem('token') || '');
  const [isAuthenticated, setIsAuthenticated] = useState(!!jwtToken);

  useEffect(() => {
    localStorage.setItem('token', jwtToken);
    setIsAuthenticated(!!jwtToken); // Update isAuthenticated immediately

  }, [jwtToken]);

  const login = (token) => {
    setJwtToken(token); // Token will now be saved to local storage
    setIsAuthenticated(true);
    //console.log(isAuthenticated);
  };

  const logout = () => {
    setJwtToken('');//Token will now be deleted from local storage
    setIsAuthenticated(false);
  };

  const value = {
    isAuthenticated,
    login,
    logout,
    token: jwtToken,
  };

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  return useContext(AuthContext);
};