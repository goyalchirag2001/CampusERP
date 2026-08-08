import { Component, OnInit, inject, signal, ChangeDetectionStrategy } from '@angular/core';

import { MatCardModule } from '@angular/material/card';

import { DashboardService } from '../services/dashboard.service';

import { DashboardResponse } from '../models/dashboard-response';

import { CurrentUserService } from '../../../core/services/current-user';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [MatCardModule],
  templateUrl: './dashboard.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrl: './dashboard.scss',
})
export class Dashboard implements OnInit {
  private readonly dashboardService = inject(DashboardService);

  private readonly currentUserService = inject(CurrentUserService);

  readonly dashboard = signal<DashboardResponse | null>(null);

  get userName(): string {
    const user = this.currentUserService.user();

    if (!user) {
      return 'User';
    }

    return `${user.firstName} ${user.lastName}`;
  }

  ngOnInit(): void {
    this.dashboardService.getDashboard().subscribe((data) => {
      this.dashboard.set(data);
    });
  }
}
