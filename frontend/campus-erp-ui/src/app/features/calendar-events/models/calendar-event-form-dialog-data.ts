import { CalendarEvent } from './calendar-event';

export interface CalendarEventFormDialogData {
  mode: 'create' | 'edit';

  event?: CalendarEvent;
}
