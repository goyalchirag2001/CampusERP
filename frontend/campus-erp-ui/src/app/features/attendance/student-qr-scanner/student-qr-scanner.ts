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

import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDividerModule } from '@angular/material/divider';

import { BrowserMultiFormatReader, IScannerControls } from '@zxing/browser';

import { AttendanceService } from '../services/attendance';

import { NotificationService } from '../../../core/services/notification';

@Component({
  selector: 'app-student-qr-scanner',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatDividerModule,
  ],
  templateUrl: './student-qr-scanner.html',
  styleUrl: './student-qr-scanner.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class StudentQrScanner implements OnDestroy {
  private readonly attendanceService = inject(AttendanceService);

  private readonly notificationService = inject(NotificationService);

  @ViewChild('video')
  private video?: ElementRef<HTMLVideoElement>;

  readonly scanning = signal(false);

  readonly submitting = signal(false);

  readonly success = signal(false);

  readonly error = signal<string | null>(null);

  private reader?: BrowserMultiFormatReader;

  private scannerControls?: IScannerControls;

  constructor() {
    this.reader = new BrowserMultiFormatReader();
  }

  ngOnDestroy(): void {
    this.stopCamera();
  }

  // =========================================================
  // Camera
  // =========================================================

  async startCamera(): Promise<void> {
    if (!this.reader || !this.video || this.scanning() || this.submitting()) {
      return;
    }

    this.error.set(null);

    this.success.set(false);

    this.scanning.set(true);

    try {
      this.scannerControls = await this.reader.decodeFromVideoDevice(
        undefined,
        this.video.nativeElement,
        (result) => {
          if (!result || this.submitting()) {
            return;
          }

          const text = result.getText();

          if (!text) {
            return;
          }

          this.handleToken(text);
        },
      );
    } catch {
      this.scanning.set(false);

      this.error.set(
        'Unable to access the camera. Please allow camera permission or upload a QR image instead.',
      );
    }
  }

  stopCamera(): void {
    this.scannerControls?.stop();

    this.scannerControls = undefined;

    this.scanning.set(false);

    const video = this.video?.nativeElement;

    if (!video) {
      return;
    }

    const stream = video.srcObject as MediaStream | null;

    if (stream) {
      stream.getTracks().forEach((track) => track.stop());
    }

    video.srcObject = null;
  }

  // =========================================================
  // Image upload
  // =========================================================

  async onFileSelected(event: Event): Promise<void> {
    const input = event.target as HTMLInputElement;

    const file = input.files?.[0];

    if (!file || !this.reader || this.submitting()) {
      input.value = '';

      return;
    }

    this.error.set(null);

    this.success.set(false);

    try {
      const imageUrl = URL.createObjectURL(file);

      const result = await this.reader.decodeFromImageUrl(imageUrl);

      URL.revokeObjectURL(imageUrl);

      if (!result) {
        this.error.set('No QR code could be detected in the selected image.');

        return;
      }

      this.handleToken(result.getText());
    } catch {
      this.error.set('Unable to read a QR code from this image.');
    } finally {
      input.value = '';
    }
  }

  // =========================================================
  // Submission
  // =========================================================

  private handleToken(token: string): void {
    this.stopCamera();

    this.submitAttendance(token);
  }

  private submitAttendance(token: string): void {
    if (this.submitting()) {
      return;
    }

    this.submitting.set(true);

    this.error.set(null);

    this.attendanceService.scanQr(token).subscribe({
      next: (response) => {
        this.submitting.set(false);

        this.success.set(true);

        this.notificationService.success(response.message || 'Attendance marked successfully.');
      },

      error: (err) => {
        this.submitting.set(false);

        this.error.set(err?.error?.message ?? err?.message ?? 'Unable to mark attendance.');
      },
    });
  }

  scanAgain(): void {
    this.success.set(false);

    this.error.set(null);

    setTimeout(() => {
      void this.startCamera();
    });
  }
}
