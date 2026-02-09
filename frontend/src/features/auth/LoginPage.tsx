import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Box, Card, CardContent, TextField, Button, Typography, Alert } from '@mui/material';
import { useAuth } from '../../contexts/AuthContext';

export default function LoginPage() {
  const [nomeUsuario, setNomeUsuario] = useState('');
  const [senha, setSenha] = useState('');
  const [error, setError] = useState('');
  const { login } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    try {
      await login({ nomeUsuario, senha });
      navigate('/');
    } catch {
      setError('Usuário ou senha inválidos');
    }
  };

  return (
    <Box display="flex" justifyContent="center" alignItems="center" minHeight="100vh" bgcolor="grey.100">
      <Card sx={{ minWidth: 400 }}>
        <CardContent>
          <Typography variant="h5" textAlign="center" mb={3}>
            Sistema de Escalas GCM
          </Typography>
          <form onSubmit={handleSubmit}>
            {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
            <TextField
              fullWidth label="Usuário" value={nomeUsuario}
              onChange={e => setNomeUsuario(e.target.value)}
              margin="normal" required autoFocus
            />
            <TextField
              fullWidth label="Senha" type="password" value={senha}
              onChange={e => setSenha(e.target.value)}
              margin="normal" required
            />
            <Button type="submit" variant="contained" fullWidth sx={{ mt: 2 }} size="large">
              Entrar
            </Button>
          </form>
        </CardContent>
      </Card>
    </Box>
  );
}
