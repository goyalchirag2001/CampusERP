import {
  Component,
  OnInit,
  computed,
  inject,
  signal,
  ChangeDetectionStrategy,
} from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { CurrentUserService } from '../../../core/services/current-user';
import { CampusService } from '../services/campus';
import { Campus } from '../models/campus';
import { MatTooltipModule } from '@angular/material/tooltip';

@Component({
  selector: 'app-campus-list',
  standalone: true,
  imports: [
    FormsModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatTableModule,
    MatPaginatorModule,
    MatTooltipModule,
  ],
  templateUrl: './campus-list.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrl: './campus-list.scss',
})
export class CampusList implements OnInit {
  private readonly campusService = inject(CampusService);

  private readonly router = inject(Router);

  private readonly currentUserService = inject(CurrentUserService);

  readonly campuses = signal<Campus[]>([]);

  readonly search = signal('');

  readonly pageSize = signal(10);

  readonly pageIndex = signal(0);

  private get baseRoute(): string {
    const slug = this.currentUserService.user()?.institutionSlug;

    return slug ? `/${slug}` : '/platform';
  }

  displayedColumns = [
    'name',
    'institution',
    'departments',
    'teachers',
    'students',
    'status',
    'actions',
  ];

  readonly filteredCampuses = computed(() =>
    this.campuses().filter(
      (x) =>
        x.name.toLowerCase().includes(this.search().toLowerCase()) ||
        x.code.toLowerCase().includes(this.search().toLowerCase()),
    ),
  );

  readonly pagedCampuses = computed(() => {
    const start = this.pageIndex() * this.pageSize();

    const end = start + this.pageSize();

    return this.filteredCampuses().slice(start, end);
  });

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.campusService.getAll().subscribe((data) => this.campuses.set(data));
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex.set(event.pageIndex);

    this.pageSize.set(event.pageSize);
  }

  createCampus(): void {
    this.router.navigate([this.baseRoute, 'campuses', 'new']);
  }

  openCampus(id: string): void {
    this.router.navigate([this.baseRoute, 'campuses', id]);
  }
}
