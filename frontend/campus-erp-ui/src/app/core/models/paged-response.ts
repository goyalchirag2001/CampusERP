import { ApiResponse } from './api-response';

export interface PagedResponse<T> extends ApiResponse<T[]> {
  pageNumber: number;

  pageSize: number;

  totalRecords: number;

  totalPages: number;
}