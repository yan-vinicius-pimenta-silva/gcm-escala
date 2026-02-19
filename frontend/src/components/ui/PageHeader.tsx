import { Box, Typography, Button } from '@mui/material';
import AddIcon from '@mui/icons-material/Add';

interface PageHeaderProps {
  title: string;
  onAdd?: () => void;
  addLabel?: string;
}

export default function PageHeader({ title, onAdd, addLabel = 'Novo' }: PageHeaderProps) {
  return (
    <Box display="flex" justifyContent="space-between" alignItems="center" mb={3} flexWrap="wrap" gap={1}>
      <Typography variant="h5" component="h1" fontWeight={600}>{title}</Typography>
      {onAdd && (
        <Button variant="contained" startIcon={<AddIcon />} onClick={onAdd}>
          {addLabel}
        </Button>
      )}
    </Box>
  );
}
