import React, { useState, useEffect } from 'react';
import LoginPage from './LoginPage';
import RpslsGamePage from './RpslsGamePage';
import { Button, Box } from '@mui/material';
import { BrowserRouter, Routes, Route, useNavigate, Navigate } from 'react-router-dom';

function App() {
  const [userId, setUserId] = useState(localStorage.getItem('userId') || '');

  useEffect(() => {
    // Keep userId in sync with localStorage
    if (userId) {
      localStorage.setItem('userId', userId);
    } else {
      localStorage.removeItem('userId');
    }
  }, [userId]);

  const handleLogout = () => {
    setUserId('');
    localStorage.removeItem('userId');
  };

  return (
    <BrowserRouter>
      <Box sx={{ minHeight: '100vh', bgcolor: '#f5f5f5', py: 4 }}>
        <Routes>
          <Route
            path="/login"
            element={
              userId ? <Navigate to="/game" replace /> : <LoginPage onLogin={setUserId} />
            }
          />
          <Route
            path="/game"
            element={
              userId ? (
                <>
                  <Box sx={{ textAlign: 'right', px: 2 }}>
                    <Button variant="outlined" color="secondary" onClick={handleLogout}>
                      Logout
                    </Button>
                  </Box>
                  <RpslsGamePage userId={userId} />
                </>
              ) : (
                <Navigate to="/login" replace />
              )
            }
          />
          <Route
            path="*"
            element={<Navigate to={userId ? "/game" : "/login"} replace />}
          />
        </Routes>
      </Box>
    </BrowserRouter>
  );
}

export default App;