import { Injectable, inject } from '@angular/core';
import { HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../../environments/environment';

import { ApiService } from '../../../core/services/api';
import { ApiEndpoints } from '../../../core/constants/api-endpoints';

import { AttendanceSession } from '../models/attendance-session';
import { AttendanceQrSession } from '../models/attendance-qr-session';
import { AttendanceQrScanResponse } from '../models/attendance-qr-scan';

export interface CreateAttendanceSessionRequest {
  timetableTemplateId: string;
  attendanceDate: string;
  lectureOverrideId?: string | null;
  remarks?: string | null;
}

export interface MarkAttendanceItem {
  attendanceRecordId: string;
  status: number;
  remarks?: string | null;
}

export interface MarkAttendanceBulkRequest {
  attendanceSessionId: string;
  records: MarkAttendanceItem[];
}

export interface CompleteAttendanceSessionRequest {
  attendanceSessionId: string;
  remarks?: string | null;
}

@Injectable({
  providedIn: 'root',
})
export class AttendanceService {
  private readonly api = inject(ApiService);

  private static readonly Endpoint = `${environment.apiUrl}/${ApiEndpoints.Attendances}`;

  // =========================================================
  // Sessions
  // =========================================================

  getSession(id: string): Observable<AttendanceSession> {
    return this.api.get<AttendanceSession>(`${AttendanceService.Endpoint}/sessions/${id}`);
  }

  getTeacherSessions(startDate: string, endDate: string): Observable<AttendanceSession[]> {
    const params = new HttpParams().set('startDate', startDate).set('endDate', endDate);

    return this.api.get<AttendanceSession[]>(
      `${AttendanceService.Endpoint}/sessions/teacher`,
      params,
    );
  }

  createSession(request: CreateAttendanceSessionRequest): Observable<AttendanceSession> {
    return this.api.post<AttendanceSession>(`${AttendanceService.Endpoint}/sessions`, request);
  }

  // =========================================================
  // Manual Attendance
  // =========================================================

  markAttendance(
    attendanceRecordId: string,
    status: number,
    remarks?: string | null,
  ): Observable<AttendanceSession> {
    return this.api.put<AttendanceSession>(`${AttendanceService.Endpoint}/records`, {
      attendanceRecordId,
      status,
      remarks: remarks ?? null,
    });
  }

  markAttendanceBulk(request: MarkAttendanceBulkRequest): Observable<AttendanceSession> {
    return this.api.put<AttendanceSession>(`${AttendanceService.Endpoint}/records/bulk`, request);
  }

  completeSession(request: CompleteAttendanceSessionRequest): Observable<AttendanceSession> {
    return this.api.post<AttendanceSession>(
      `${AttendanceService.Endpoint}/sessions/complete`,
      request,
    );
  }

  lockSession(id: string): Observable<AttendanceSession> {
    return this.api.post<AttendanceSession>(
      `${AttendanceService.Endpoint}/sessions/${id}/lock`,
      {},
    );
  }

  // =========================================================
  // QR Attendance
  // =========================================================

  startQr(attendanceSessionId: string, durationSeconds: number): Observable<AttendanceQrSession> {
    return this.api.post<AttendanceQrSession>(`${AttendanceService.Endpoint}/qr/start`, {
      attendanceSessionId,
      durationSeconds,
    });
  }

  getActiveQr(attendanceSessionId: string): Observable<AttendanceQrSession> {
    return this.api.get<AttendanceQrSession>(
      `${AttendanceService.Endpoint}/sessions/${attendanceSessionId}/qr`,
    );
  }

  scanQr(token: string): Observable<AttendanceQrScanResponse> {
    return this.api.post<AttendanceQrScanResponse>(`${AttendanceService.Endpoint}/qr/scan`, {
      token,
    });
  }

  closeQr(attendanceSessionId: string): Observable<AttendanceQrSession> {
    return this.api.post<AttendanceQrSession>(
      `${AttendanceService.Endpoint}/sessions/${attendanceSessionId}/qr/close`,
      {},
    );
  }
}
