export interface LookupPickerItem {
  id: string;

  title: string;

  subtitle?: string;

  tag?: string;

  disabled?: boolean;

  payload?: any;
}

export interface LookupPickerData {
  title: string;

  placeholder?: string;

  items: LookupPickerItem[];
}
