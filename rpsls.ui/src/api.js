// import axios from 'axios';

// const API_BASE = 'http://localhost:5000/api';

// export const getChoices = () => axios.get(`${API_BASE}/choices`);
// export const getRandomChoice = () => axios.get(`${API_BASE}/choice`);
// export const playRound = (userId, playerChoice) =>
//   axios.post(`${API_BASE}/play`, { userId, playerChoice });
// export const getScoreboard = (userId) =>
//   axios.get(`${API_BASE}/scoreboard`, { params: { userId } });
// export const resetScoreboard = (userId) =>
//   axios.post(`${API_BASE}/scoreboard/reset`, null, { params: { userId } });

// export const getUsers = () => axios.get(`${API_BASE}/users`);
// export const getUser = (id) => axios.get(`${API_BASE}/users/${id}`);
// export const createUser = (username, hashedPassword) =>
//   axios.post(`${API_BASE}/users`, { username, hashedPassword });
// export const updateUser = (id, username, hashedPassword) =>
//   axios.put(`${API_BASE}/users/${id}`, { id, username, hashedPassword });
// export const deleteUser = (id) => axios.delete(`${API_BASE}/users/${id}`);