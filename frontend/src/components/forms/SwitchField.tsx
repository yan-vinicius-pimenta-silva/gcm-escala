import { FormControlLabel, Switch } from '@mui/material';
import { Controller, type Control, type FieldValues, type Path } from 'react-hook-form';

interface SwitchFieldProps<T extends FieldValues> {
  name: Path<T>;
  control: Control<T>;
  label: string;
}

export default function SwitchField<T extends FieldValues>({ name, control, label }: SwitchFieldProps<T>) {
  return (
    <Controller
      name={name}
      control={control}
      render={({ field }) => (
        <FormControlLabel
          control={<Switch checked={field.value} onChange={field.onChange} />}
          label={label}
          sx={{ mt: 1 }}
        />
      )}
    />
  );
}
