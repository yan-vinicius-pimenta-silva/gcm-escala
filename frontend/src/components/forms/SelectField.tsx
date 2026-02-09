import { FormControl, InputLabel, Select, MenuItem, FormHelperText } from '@mui/material';
import { Controller, type Control, type FieldValues, type Path } from 'react-hook-form';

interface Option {
  value: string | number;
  label: string;
}

interface SelectFieldProps<T extends FieldValues> {
  name: Path<T>;
  control: Control<T>;
  label: string;
  options: Option[];
  disabled?: boolean;
}

export default function SelectField<T extends FieldValues>({
  name, control, label, options, disabled,
}: SelectFieldProps<T>) {
  return (
    <Controller
      name={name}
      control={control}
      render={({ field, fieldState: { error } }) => (
        <FormControl fullWidth margin="normal" error={!!error}>
          <InputLabel>{label}</InputLabel>
          <Select {...field} label={label} disabled={disabled}>
            {options.map((opt) => (
              <MenuItem key={opt.value} value={opt.value}>{opt.label}</MenuItem>
            ))}
          </Select>
          {error && <FormHelperText>{error.message}</FormHelperText>}
        </FormControl>
      )}
    />
  );
}
