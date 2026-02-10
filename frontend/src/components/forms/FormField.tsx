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
  maxLength?: number;
}

export default function FormField<T extends FieldValues>({
  name, control, label, type = 'text', multiline, rows, disabled, maxLength = 144,
}: FormFieldProps<T>) {
  const shouldShrinkLabel = type === 'date' || type === 'time' || type === 'datetime-local';

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
          inputProps={{ maxLength }}
          InputLabelProps={shouldShrinkLabel ? { shrink: true } : undefined}
        />
      )}
    />
  );
}
