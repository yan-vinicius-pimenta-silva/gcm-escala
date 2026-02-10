export function formatDateToDisplay(value: unknown): string {
  if (typeof value !== 'string' || value.trim() === '') return '';

  const [datePart] = value.split('T');
  const match = datePart.match(/^(\d{4})-(\d{2})-(\d{2})$/);

  if (!match) return value;

  const [, year, month, day] = match;
  return `${day}-${month}-${year}`;
}

export function isIsoDateString(value: unknown): value is string {
  if (typeof value !== 'string') return false;
  const [datePart] = value.split('T');
  return /^\d{4}-\d{2}-\d{2}$/.test(datePart);
}
