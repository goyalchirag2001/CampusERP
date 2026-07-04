import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { CurrentUserService } from '../../../core/services/current-user';
import { SubjectService } from '../services/subject';
import { Subject } from '../models/subject';
import { SubjectCreateDialog } from '../subject-create-dialog/subject-create-dialog';

@Component({
  selector: 'app-subject-list',
  standalone: true,
  imports: [
    FormsModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatTableModule,
    MatPaginatorModule,
    MatDialogModule,
  ],
  templateUrl: './subject-list.html',
  styleUrl: './subject-list.scss',
})
export class SubjectList implements OnInit {
  private readonly subjectService = inject(SubjectService);

  private readonly currentUserService = inject(CurrentUserService);

  private readonly router = inject(Router);

  private readonly dialog = inject(MatDialog);

  readonly subjects = signal<Subject[]>([]);

  readonly search = signal('');

  readonly pageSize = signal(10);

  readonly pageIndex = signal(0);

  displayedColumns = ['name', 'code', 'credits', 'type', 'status', 'actions'];

  private get baseRoute(): string {
    const slug = this.currentUserService.user()?.institutionSlug;

    return slug ? `/${slug}` : '/platform';
  }

  readonly filteredSubjects = computed(() =>
    this.subjects().filter(
      (x) =>
        x.name.toLowerCase().includes(this.search().toLowerCase()) ||
        x.code.toLowerCase().includes(this.search().toLowerCase()),
    ),
  );

  readonly pagedSubjects = computed(() => {
    const start = this.pageIndex() * this.pageSize();

    const end = start + this.pageSize();

    return this.filteredSubjects().slice(start, end);
  });

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.subjectService.getAll().subscribe((data) => {
      this.subjects.set(data);
    });
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex.set(event.pageIndex);

    this.pageSize.set(event.pageSize);
  }

  createSubject(): void {
    this.dialog
      .open(SubjectCreateDialog, {
        width: '650px',
        maxWidth: '95vw',
      })
      .afterClosed()
      .subscribe((created) => {
        if (created) {
          this.load();
        }
      });
  }

  openSubject(id: string): void {
    this.router.navigate([this.baseRoute, 'subjects', id]);
  }

  getTypeName(type: number): string {
    switch (type) {
      case 1:
        return 'Core';

      case 2:
        return 'Elective';

      case 3:
        return 'Laboratory';

      case 4:
        return 'Project';

      default:
        return '-';
    }
  }
}
