import { StudentImportCredential } from './student-import-credential';
import { StudentImportError } from './student-import-error';
import { StudentImportPreview } from './student-import-preview';

export interface StudentImportValidation {
  totalRows: number;

  validRows: number;

  invalidRows: number;

  canImport: boolean;

  preview: StudentImportPreview[];

  errors: StudentImportError[];

  credentials: StudentImportCredential[];
}