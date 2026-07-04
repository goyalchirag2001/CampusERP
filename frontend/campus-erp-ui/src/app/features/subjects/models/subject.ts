export interface Subject {
  id: string;

  institutionId: string;

  campusId: string;

  campusName?: string;

  code: string;

  name: string;

  credits: number;

  subjectType: number;

  isActive: boolean;
}
