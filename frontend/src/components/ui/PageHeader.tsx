import { Box, Typography, Button } from '@mui/material';
import AddIcon from '@mui/icons-material/Add';

interface PageHeaderProps {
  title: string;
  onAdd?: () => void;
  addLabel?: string;
}

export default function PageHeader({ title, onAdd, addLabel = 'Novo' }: PageHeaderProps) {
  return (
    <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
      <Typography variant="h4">{title}</Typography>
      {onAdd && (
        <Button variant="contained" startIcon={<AddIcon />} onClick={onAdd}>
          {addLabel}
        </Button>
      )}
    </Box>
  );
}
