import { Controller, type Control, type FieldValues, type Path } from 'react-hook-form';
import { TimeField } from '@mui/x-date-pickers/TimeField';
import dayjs, { type Dayjs } from 'dayjs';

interface TimeInputFieldProps<T extends FieldValues> {
  name: Path<T>;
  control: Control<T>;
  label: string;
  disabled?: boolean;
}

export default function TimeInputField<T extends FieldValues>({
  name,
  control,
  label,
  disabled,
}: TimeInputFieldProps<T>) {
  return (
    <Controller
      name={name}
      control={control}
      render={({ field, fieldState: { error } }) => {
        const dayjsValue = field.value ? dayjs(field.value, 'HH:mm') : null;
        return (
          <TimeField
            label={label}
            value={dayjsValue}
            onChange={(newValue: Dayjs | null) => {
              field.onChange(newValue && newValue.isValid() ? newValue.format('HH:mm') : '');
            }}
            format="HH:mm"
            disabled={disabled}
            slotProps={{
              textField: {
                fullWidth: true,
                margin: 'normal',
                error: !!error,
                helperText: error?.message,
                onBlur: field.onBlur,
              },
            }}
          />
        );
      }}
    />
  );
}
