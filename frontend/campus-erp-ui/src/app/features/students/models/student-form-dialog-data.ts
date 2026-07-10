import { Student } from '../models/student';

export interface StudentFormDialogData {
  mode: 'create' | 'edit';

  student?: Student;
}
