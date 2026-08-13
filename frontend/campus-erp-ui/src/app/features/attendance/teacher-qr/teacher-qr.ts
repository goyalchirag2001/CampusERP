import {
  ChangeDetectionStrategy,
  Component,
  ElementRef,
  OnDestroy,
  ViewChild,
  inject,
  signal,
} from '@angular/core';

import { CommonModule } from '@angular/common';

import { ActivatedRoute, Router } from '@angular/router';

import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatDividerModule } from '@angular/material/divider';

import * as QRCode from 'qrcode';

import { AttendanceService } from '../services/attendance';
import { AttendanceQrSession } from '../models/attendance-qr-session';

import { NotificationService } from '../../../core/services/notification';

@Component({
  selector: 'app-teacher-qr',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatTooltipModule,
    MatSelectModule,
    MatFormFieldModule,
    MatDividerModule,
  ],
  templateUrl: './teacher-qr.html',
  styleUrl: './teacher-qr.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TeacherQr implements OnDestroy {
  private readonly route = inject(ActivatedRoute);

  private readonly router = inject(Router);

  private readonly attendanceService = inject(AttendanceService);

  private readonly notificationService = inject(NotificationService);

  @ViewChild('qrCanvas')
  private qrCanvas?: ElementRef<HTMLCanvasElement>;

  readonly loading = signal(false);

  readonly starting = signal(false);

  readonly closing = signal(false);

  readonly refreshing = signal(false);

  readonly qrSession = signal<AttendanceQrSession | null>(null);

  readonly selectedDuration = signal(60);

  readonly remainingSeconds = signal(0);

  readonly error = signal<string | null>(null);

  private countdownTimer: ReturnType<typeof setInterval> | null = null;

  private refreshTimer: ReturnType<typeof setInterval> | null = null;

  private renderTimer: ReturnType<typeof setTimeout> | null = null;

  private get attendanceSessionId(): string | null {
    return this.route.snapshot.paramMap.get('id');
  }

  // =========================================================
  // Lifecycle
  // =========================================================

  ngOnInit(): void {
    this.loadExistingQr();
  }

  ngOnDestroy(): void {
    this.stopTimers();
  }

  // =========================================================
  // Initial QR
  // =========================================================

  private loadExistingQr(): void {
    const sessionId = this.attendanceSessionId;

    if (!sessionId) {
      this.error.set('Attendance session ID was not provided.');

      return;
    }

    this.loading.set(true);

    this.attendanceService.getActiveQr(sessionId).subscribe({
      next: (qr) => {
        this.loading.set(false);

        this.setQrSession(qr);
      },

      error: () => {
        /*
         * No active QR is a valid initial state.
         * The teacher simply hasn't started QR attendance.
         */
        this.loading.set(false);

        this.qrSession.set(null);
      },
    });
  }

  // =========================================================
  // Start QR
  // =========================================================

  startQr(): void {
    const sessionId = this.attendanceSessionId;

    if (!sessionId || this.starting()) {
      return;
    }

    this.starting.set(true);

    this.error.set(null);

    this.attendanceService.startQr(sessionId, this.selectedDuration()).subscribe({
      next: (qr) => {
        this.starting.set(false);

        this.setQrSession(qr);

        this.notificationService.success('QR attendance started.');
      },

      error: (err) => {
        this.starting.set(false);

        const message = err?.error?.message ?? err?.message ?? 'Unable to start QR attendance.';

        this.error.set(message);

        this.notificationService.error(message);
      },
    });
  }

  // =========================================================
  // Close QR
  // =========================================================

  closeQr(): void {
    const sessionId = this.attendanceSessionId;

    if (!sessionId || this.closing()) {
      return;
    }

    const confirmed = window.confirm(
      'Close QR attendance now? Students who have not scanned will be marked absent.',
    );

    if (!confirmed) {
      return;
    }

    this.closing.set(true);

    this.attendanceService.closeQr(sessionId).subscribe({
      next: (qr) => {
        this.closing.set(false);

        this.setQrSession({
          ...qr,
          isActive: false,
        });

        this.stopTimers();

        this.notificationService.success(
          'QR attendance closed. Remaining students were marked absent.',
        );
      },

      error: (err) => {
        this.closing.set(false);

        this.notificationService.error(
          err?.error?.message ?? err?.message ?? 'Unable to close QR attendance.',
        );
      },
    });
  }

  // =========================================================
  // QR state
  // =========================================================

  private setQrSession(qr: AttendanceQrSession): void {
    this.qrSession.set(qr);

    this.updateCountdown();

    if (qr.isActive) {
      this.startTimers();
    } else {
      this.stopTimers();
    }

    this.scheduleQrRender();
  }

  private scheduleQrRender(): void {
    if (this.renderTimer) {
      clearTimeout(this.renderTimer);
    }

    this.renderTimer = setTimeout(() => {
      this.renderTimer = null;

      void this.renderQr();
    });
  }

  // =========================================================
  // Countdown
  // =========================================================

  private startTimers(): void {
    this.stopTimers();

    this.updateCountdown();

    this.countdownTimer = setInterval(() => {
      this.updateCountdown();
    }, 1000);

    /*
     * Refresh counts from the server every 3 seconds.
     */
    this.refreshTimer = setInterval(() => {
      this.refreshQrStatus();
    }, 3000);
  }

  private updateCountdown(): void {
    const qr = this.qrSession();

    if (!qr) {
      this.remainingSeconds.set(0);

      return;
    }

    const expiresAt = new Date(qr.expiresOn).getTime();

    const now = Date.now();

    const remaining = Math.max(0, Math.ceil((expiresAt - now) / 1000));

    this.remainingSeconds.set(remaining);

    if (remaining === 0 && qr.isActive) {
      this.handleQrExpired();
    }
  }

  private handleQrExpired(): void {
    this.stopTimers();

    const current = this.qrSession();

    if (!current) {
      return;
    }

    this.qrSession.set({
      ...current,
      isActive: false,
    });

    /*
     * The backend background service is the authority
     * for marking remaining students absent.
     *
     * We refresh once more so the teacher sees the final
     * server-side count.
     */
    setTimeout(() => {
      this.refreshQrStatus(true);
    }, 1500);
  }

  // =========================================================
  // Refresh
  // =========================================================

  private refreshQrStatus(allowExpired = false): void {
    const sessionId = this.attendanceSessionId;

    if (!sessionId || this.refreshing()) {
      return;
    }

    this.refreshing.set(true);

    this.attendanceService.getActiveQr(sessionId).subscribe({
      next: (qr) => {
        this.refreshing.set(false);

        this.setQrSession(qr);
      },

      error: () => {
        this.refreshing.set(false);

        const current = this.qrSession();

        if (current && allowExpired) {
          this.qrSession.set({
            ...current,
            isActive: false,
          });
        }

        if (allowExpired || this.remainingSeconds() <= 0) {
          this.stopTimers();
        }
      },
    });
  }

  // =========================================================
  // QR Rendering
  // =========================================================

  private async renderQr(): Promise<void> {
    const qr = this.qrSession();

    const canvas = this.qrCanvas?.nativeElement;

    if (!qr || !canvas) {
      return;
    }

    try {
      await QRCode.toCanvas(canvas, qr.token, {
        width: 420,
        margin: 2,
        errorCorrectionLevel: 'H',
        color: {
          dark: '#000000',
          light: '#ffffff',
        },
      });
    } catch {
      this.notificationService.error('Unable to generate the QR code.');
    }
  }

  // =========================================================
  // UI helpers
  // =========================================================

  hasQr(): boolean {
    return this.qrSession() !== null;
  }

  isQrActive(): boolean {
    const qr = this.qrSession();

    return !!qr && qr.isActive && this.remainingSeconds() > 0;
  }

  formatRemainingTime(): string {
    const total = this.remainingSeconds();

    const minutes = Math.floor(total / 60);

    const seconds = total % 60;

    return `${minutes}:${seconds.toString().padStart(2, '0')}`;
  }

  formatTime(value: string): string {
    if (!value) {
      return '—';
    }

    const date = new Date(value);

    if (Number.isNaN(date.getTime())) {
      return value;
    }

    return date.toLocaleTimeString([], {
      hour: 'numeric',
      minute: '2-digit',
      second: '2-digit',
    });
  }

  goBack(): void {
    this.router.navigate(['../'], {
      relativeTo: this.route,
    });
  }

  private stopTimers(): void {
    if (this.countdownTimer) {
      clearInterval(this.countdownTimer);

      this.countdownTimer = null;
    }

    if (this.refreshTimer) {
      clearInterval(this.refreshTimer);

      this.refreshTimer = null;
    }
  }
}
