import React, { useState } from 'react';
import { Container, Card, CardContent, Typography, TextField, Button, Alert, Box } from '@mui/material';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';

//const API_BASE = 'https://localhost:5001/api';
const API_BASE = process.env.REACT_APP_API_BASE;

const LoginPage = ({ onLogin }) => {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [info, setInfo] = useState('');
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();

  const handleLogin = async () => {
    setError('');
    setInfo('');
    if (!username || !password) {
      setError('Username and password are required.');
      return;
    }
    setLoading(true);
    try {
      const res = await axios.post(`${API_BASE}/users/login`, {
        username,
        password: password
      });
      setInfo('Login successful!');
      localStorage.setItem('userId', res.data.id);
      if (onLogin) onLogin(res.data.id);
      navigate('/game');
    } catch (err) {
      if (err.response && err.response.status === 401) {
        setError('Invalid username or password. You can register below.');
      } else {
        setError('Login failed.');
      }
    }
    setLoading(false);
  };

  const handleRegister = async () => {
    setError('');
    setInfo('');
    if (!username || !password) {
      setError('Username and password are required.');
      return;
    }
    setLoading(true);
    try {
      const res = await axios.post(`${API_BASE}/users/register`, {
        username,
        password: password
      });
      setInfo('Registration successful! You are now logged in.');
      localStorage.setItem('userId', res.data.id);
      if (onLogin) onLogin(res.data.id);
      navigate('/game');
    } catch (err) {
      if (err.response && err.response.status === 409) {
        setError('Username already exists.');
      } else {
        setError('Registration failed.');
      }
    }
    setLoading(false);
  };

  return (
    <Container maxWidth="sm" sx={{ mt: 4 }}>
      <Card variant="outlined">
        <CardContent>
          <Typography variant="h5" align="center" gutterBottom>
            Login
          </Typography>
          {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
          {info && <Alert severity="success" sx={{ mb: 2 }}>{info}</Alert>}
          <TextField
            label="Username"
            fullWidth
            margin="normal"
            value={username}
            onChange={e => setUsername(e.target.value)}
            disabled={loading}
          />
          <TextField
            label="Password"
            type="password"
            fullWidth
            margin="normal"
            value={password}
            onChange={e => setPassword(e.target.value)}
            disabled={loading}
          />
          <Box sx={{ display: 'flex', gap: 2, mt: 2 }}>
            <Button
              variant="contained"
              color="primary"
              fullWidth
              onClick={handleLogin}
              disabled={loading}
            >
              Login
            </Button>
            <Button
              variant="outlined"
              color="secondary"
              fullWidth
              onClick={handleRegister}
              disabled={loading}
            >
              Register
            </Button>
          </Box>
        </CardContent>
      </Card>
    </Container>
  );
};

export default LoginPage;