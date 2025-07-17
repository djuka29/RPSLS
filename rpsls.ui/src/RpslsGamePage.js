import React, { useEffect, useState } from 'react';
import {
    Container, Typography, Box, Button, Select, MenuItem, Card, CardContent, Grid, Alert, Divider, InputLabel, FormControl
} from '@mui/material';
import ReplayIcon from '@mui/icons-material/Replay';
import SportsEsportsIcon from '@mui/icons-material/SportsEsports';
import ContentCutIcon from '@mui/icons-material/ContentCut';
import DescriptionIcon from '@mui/icons-material/Description';
import ShuffleIcon from '@mui/icons-material/Shuffle';
import CircleIcon from '@mui/icons-material/Circle';
import PetsIcon from '@mui/icons-material/Pets';
import PanToolAltIcon from '@mui/icons-material/PanToolAlt';
import { useNavigate } from 'react-router-dom';

import axios from 'axios';

// API base URL
//const API_BASE = 'https://localhost:5001/api';
const API_BASE = process.env.REACT_APP_API_BASE;

// Enum mapping for RoundResultType
const ROUND_RESULT_MAP = {
    0: 'Tie',
    1: 'Win',
    2: 'Lose'
};

// Color mapping for results
const RESULT_COLOR_MAP = {
    0: 'text.secondary', // Tie - gray
    1: 'success.main',   // Win - green
    2: 'error.main'      // Lose - red
};

// Icon mapping for choices
const CHOICE_ICONS = {
    rock: <CircleIcon sx={{ mr: 1, color: '#757575' }} />,
    paper: <DescriptionIcon sx={{ mr: 1, color: '#1976d2' }} />,
    scissors: <ContentCutIcon sx={{ mr: 1, color: '#d32f2f' }} />,
    lizard: <PetsIcon sx={{ mr: 1, color: '#43a047' }} />,
    spock: <PanToolAltIcon sx={{ mr: 1, color: '#fbc02d' }} />
};

const RpslsGamePage = ({ userId }) => {
    const navigate = useNavigate();
    const [choices, setChoices] = useState([]);
    const [playerChoice, setPlayerChoice] = useState('');
    const [result, setResult] = useState(null);
    const [scoreboard, setScoreboard] = useState([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');

    // On mount, check for userId in localStorage
    useEffect(() => {
        if (!userId) {
            navigate('/login');
        }
    }, [userId, navigate]);

    // Fetch choices on mount
    useEffect(() => {
        axios.get(`${API_BASE}/choices`)
            .then(res => setChoices(res.data))
            .catch(() => setError('Failed to load choices.'));
    }, []);

    // Fetch scoreboard when userId changes or after play
    useEffect(() => {
        if (userId) {
            axios.get(`${API_BASE}/scoreboard`, { params: { userId } })
                .then(res => setScoreboard(res.data))
                .catch(() => setError('Failed to load scoreboard.'));
        }
    }, [userId, result]);

    const handlePlay = async () => {
        if (!playerChoice) {
            setError('Please select your choice.');
            return;
        }
        setLoading(true);
        setError('');
        try {
            const res = await axios.post(`${API_BASE}/play`, {
                userId,
                playerChoice: playerChoice
            });
            setResult(res.data);
        } catch {
            setError('Failed to play round.');
        }
        setLoading(false);
    };

    const handleResetScoreboard = async () => {
        setLoading(true);
        setError('');
        try {
            await axios.post(`${API_BASE}/scoreboard/reset`, null, { params: { userId } });
            setScoreboard([]);
            setResult(null);
        } catch {
            setError('Failed to reset scoreboard.');
        }
        setLoading(false);
    };

    const getChoiceIcon = (name) => CHOICE_ICONS[name?.toLowerCase()] || null;

    // Handle random select choice
    const handleRandomChoice = async () => {
        setLoading(true);
        setError('');
        try {
            const res = await axios.get(`${API_BASE}/randomchoice`);
            if (res.data && res.data.id) {
                setPlayerChoice(res.data.id);
            } else {
                setError('Failed to get random choice.');
            }
        } catch {
            setError('Failed to get random choice.');
        }
        setLoading(false);
    };

    return (
        <Container maxWidth="lg" sx={{ mt: 6 }}>
            <Card variant="outlined" sx={{ mb: 3, boxShadow: 4, borderRadius: 4, px: 4, py: 2 }}>
                <CardContent>
                    <Typography variant="h3" align="center" gutterBottom sx={{ fontWeight: 'bold', letterSpacing: 2 }}>
                        Rock Paper Scissors Lizard Spock
                    </Typography>
                    <Typography variant="body1" align="center" color="text.secondary" gutterBottom sx={{ mb: 2 }}>
                        Select your choice and play against the computer!
                    </Typography>
                    <Divider sx={{ mb: 3 }} />
                    {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
                    <Box
                        sx={{
                            display: 'flex',
                            justifyContent: 'center',
                            alignItems: 'flex-start',
                            mb: 2,
                            gap: 4,
                            flexWrap: { xs: 'wrap', sm: 'nowrap' }
                        }}
                    >
                        <Box sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center', mr: 2, gap: 1 }}>
                            <FormControl sx={{ minWidth: 220, bgcolor: '#f5f5f5', borderRadius: 1 }}>
                                <Select
                                    value={playerChoice}
                                    onChange={e => setPlayerChoice(e.target.value)}
                                    displayEmpty
                                    sx={{ fontWeight: 'bold', fontSize: 18, height: 56 }}
                                >
                                    <MenuItem value="">
                                        <Box sx={{ display: 'flex', alignItems: 'center' }}>
                                            <SportsEsportsIcon sx={{ mr: 1, color: '#bdbdbd' }} />
                                            <em>Select Choice</em>
                                        </Box>
                                    </MenuItem>
                                    {choices.map(c => (
                                        <MenuItem key={c.id} value={c.id}>
                                            <Box sx={{ display: 'flex', alignItems: 'center' }}>
                                                {getChoiceIcon(c.name)}
                                                {c.name.charAt(0).toUpperCase() + c.name.slice(1)}
                                            </Box>
                                        </MenuItem>
                                    ))}
                                </Select>
                            </FormControl>
                            <Button
                                variant="outlined"
                                color="secondary"
                                onClick={handleRandomChoice}
                                disabled={loading || !userId}
                                sx={{ fontWeight: 'bold', px: 3, py: 1.5, height: 48, fontSize: 16, mt: 1, width: '100%' }}
                                startIcon={<ShuffleIcon />}
                            >
                                Random Choice
                            </Button>
                        </Box>
                        <Box sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 1 }}>
                            <Button
                                variant="contained"
                                color="primary"
                                onClick={handlePlay}
                                disabled={loading || !userId}
                                sx={{ fontWeight: 'bold', px: 4, py: 1.5, boxShadow: 2, height: 56, fontSize: 18, width: 260 }}
                                startIcon={<SportsEsportsIcon />}
                            >
                                Play
                            </Button>
                            <Button
                                variant="outlined"
                                color="secondary"
                                onClick={handleResetScoreboard}
                                disabled={loading || !userId}
                                sx={{
                                    fontWeight: 'bold',
                                    px: 3,
                                    py: 1.5,
                                    height: 48,
                                    fontSize: 16,
                                    textTransform: 'uppercase',
                                    mt: 1,
                                    width: 260
                                }}
                                startIcon={<ReplayIcon />}
                            >
                                Reset Scoreboard
                            </Button>
                        </Box>
                    </Box>
                    {result && (
                        <Box sx={{ textAlign: 'center', mb: 3 }}>
                            <Typography variant="h6" sx={{ mb: 1 }}>
                                You chose: <strong>
                                    {getChoiceIcon(choices.find(c => c.id === result.player)?.name)}
                                    {choices.find(c => c.id === result.player)?.name}
                                </strong>
                            </Typography>
                            <Typography variant="h6" sx={{ mb: 1 }}>
                                Computer chose: <strong>
                                    {getChoiceIcon(choices.find(c => c.id === result.computer)?.name)}
                                    {choices.find(c => c.id === result.computer)?.name}
                                </strong>
                            </Typography>
                            <Typography
                                variant="h5"
                                sx={{
                                    fontWeight: 'bold',
                                    mb: 1,
                                    color:
                                        result.result === 1
                                            ? '#2e7d32'
                                            : result.result === 2
                                                ? '#d32f2f'
                                                : '#757575'
                                }}
                            >
                                Result: {ROUND_RESULT_MAP[result.result]}
                            </Typography>
                        </Box>
                    )}
                    <Divider sx={{ mb: 2 }} />
                    <Typography variant="h6" sx={{ mt: 2, mb: 1, textAlign: 'center', fontWeight: 'bold' }}>
                        Scoreboard (Last 10 Rounds)
                    </Typography>
                    <Grid container spacing={2} justifyContent="center" sx={{ width: '100%', margin: 0 }}>
                        {scoreboard.length === 0 && (
                            <Grid item xs={12}>
                                <Typography color="text.secondary" align="center">No rounds played yet.</Typography>
                            </Grid>
                        )}
                        {/* Show last round at the top if available */}
                        {result && (
                            <Grid item xs={12} sm={6} sx={{ display: 'flex', justifyContent: 'center', width: '100%' }}>
                                <Card
                                    variant="outlined"
                                    sx={{
                                        mb: 1,
                                        borderLeft: `8px solid ${
                                            result.result === 1 ? '#2e7d32' : result.result === 2 ? '#d32f2f' : '#757575'
                                        }`,
                                        bgcolor: '#fffde7',
                                        borderRadius: 2,
                                        boxShadow: 4,
                                        width: '100%',
                                        maxWidth: '100%',
                                        minWidth: 0,
                                        mx: 0,
                                        display: 'flex',
                                        alignItems: 'center',
                                        height: 72
                                    }}
                                >
                                    <CardContent
                                        sx={{
                                            py: 1,
                                            px: 2,
                                            width: '100%',
                                            display: 'flex',
                                            alignItems: 'center',
                                            justifyContent: 'space-between'
                                        }}
                                    >
                                        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, width: '100%' }}>
                                            <Typography sx={{ fontWeight: 'bold', minWidth: 60 }}>You:</Typography>
                                            {getChoiceIcon(choices.find(c => c.id === result.player)?.name)}
                                            <Typography sx={{ minWidth: 70 }}>
                                                {choices.find(c => c.id === result.player)?.name}
                                            </Typography>
                                            <Typography sx={{ fontWeight: 'bold', minWidth: 100, ml: 2 }}>Computer:</Typography>
                                            {getChoiceIcon(choices.find(c => c.id === result.computer)?.name)}
                                            <Typography sx={{ minWidth: 70 }}>
                                                {choices.find(c => c.id === result.computer)?.name}
                                            </Typography>
                                            <Typography
                                                sx={{
                                                    fontWeight: 'bold',
                                                    ml: 2,
                                                    color:
                                                        result.result === 1
                                                            ? '#2e7d32'
                                                            : result.result === 2
                                                                ? '#d32f2f'
                                                                : '#757575',
                                                    minWidth: 90
                                                }}
                                            >
                                                Result: {ROUND_RESULT_MAP[result.result]}
                                            </Typography>
                                            <Typography sx={{ ml: 2, fontWeight: 'bold', color: '#fbc02d' }}>
                                                (Last Round)
                                            </Typography>
                                        </Box>
                                    </CardContent>
                                </Card>
                            </Grid>
                        )}
                        {scoreboard.map((r, idx) => {
                            // Skip the first scoreboard record if it matches the last result
                            const isDuplicateLastRound =
                                idx === 0 &&
                                result &&
                                r.player === result.player &&
                                r.computer === result.computer &&
                                r.result === result.result;

                            if (isDuplicateLastRound) return null;

                            const playerName = choices.find(c => c.id === r.player)?.name;
                            const computerName = choices.find(c => c.id === r.computer)?.name;
                            return (
                                <Grid
                                    item
                                    xs={12}
                                    sm={6}
                                    key={idx}
                                    sx={{
                                        display: 'flex',
                                        justifyContent: 'center',
                                        width: '100%',
                                    }}
                                >
                                    <Card
                                        variant="outlined"
                                        sx={{
                                            mb: 1,
                                            borderLeft: `8px solid ${r.result === 1 ? '#2e7d32' : r.result === 2 ? '#d32f2f' : '#757575'}`,
                                            bgcolor: '#fafafa',
                                            borderRadius: 2,
                                            boxShadow: 2,
                                            width: '100%',
                                            maxWidth: '100%',
                                            minWidth: 0,
                                            mx: 0,
                                            display: 'flex',
                                            alignItems: 'center',
                                            height: 64
                                        }}
                                    >
                                        <CardContent
                                            sx={{
                                                py: 1,
                                                px: 2,
                                                width: '100%',
                                                display: 'flex',
                                                alignItems: 'center',
                                                justifyContent: 'space-between'
                                            }}
                                        >
                                            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, width: '100%' }}>
                                                <Typography sx={{ fontWeight: 'bold', minWidth: 60 }}>You:</Typography>
                                                {getChoiceIcon(playerName)}
                                                <Typography sx={{ minWidth: 70 }}>{playerName}</Typography>
                                                <Typography sx={{ fontWeight: 'bold', minWidth: 100, ml: 2 }}>Computer:</Typography>
                                                {getChoiceIcon(computerName)}
                                                <Typography sx={{ minWidth: 70 }}>{computerName}</Typography>
                                                <Typography
                                                    sx={{
                                                        fontWeight: 'bold',
                                                        ml: 2,
                                                        color: r.result === 1 ? '#2e7d32' : r.result === 2 ? '#d32f2f' : '#757575',
                                                        minWidth: 90
                                                    }}
                                                >
                                                    Result: {ROUND_RESULT_MAP[r.result]}
                                                </Typography>
                                            </Box>
                                        </CardContent>
                                    </Card>
                                </Grid>
                            );
                        })}
                    </Grid>
                </CardContent>
            </Card>
        </Container>
    );
};

export default RpslsGamePage;