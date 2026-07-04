export interface CreateSubjectRequest {
  institutionId: string;

  campusId: string;

  code: string;

  name: string;

  credits: number;

  subjectType: number;
}
