import { TextField } from '@mui/material';
import { Controller, type Control, type FieldValues, type Path } from 'react-hook-form';

interface FormFieldProps<T extends FieldValues> {
  name: Path<T>;
  control: Control<T>;
  label: string;
  type?: string;
  multiline?: boolean;
  rows?: number;
  disabled?: boolean;
}

export default function FormField<T extends FieldValues>({
  name, control, label, type = 'text', multiline, rows, disabled,
}: FormFieldProps<T>) {
  return (
    <Controller
      name={name}
      control={control}
      render={({ field, fieldState: { error } }) => (
        <TextField
          {...field}
          label={label}
          type={type}
          fullWidth
          margin="normal"
          error={!!error}
          helperText={error?.message}
          multiline={multiline}
          rows={rows}
          disabled={disabled}
        />
      )}
    />
  );
}
