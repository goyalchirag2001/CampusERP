import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatTableModule } from '@angular/material/table';
import { InstitutionService } from '../services/institution';
import { Institution } from '../../../core/models/institution';
import { CurrentUserService } from '../../../core/services/current-user';
import { MatPaginatorModule } from '@angular/material/paginator';
import { PageEvent } from '@angular/material/paginator';
import { MatTooltipModule } from '@angular/material/tooltip';

@Component({
  selector: 'app-institution-list',
  standalone: true,
  imports: [
    FormsModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatTableModule,
    MatPaginatorModule,
    MatTooltipModule
  ],
  templateUrl: './institution-list.html',
  styleUrl: './institution-list.scss',
})
export class InstitutionList implements OnInit {
  private readonly institutionService = inject(InstitutionService);

  private readonly router = inject(Router);

  private readonly currentUserService = inject(CurrentUserService);

  readonly institutions = signal<Institution[]>([]);

  readonly search = signal('');

  readonly pageSize = signal(10);

  readonly pageIndex = signal(0);

  private get baseRoute(): string {
    const slug = this.currentUserService.user()?.institutionSlug;

    return slug ? `/${slug}` : '/platform';
  }

  displayedColumns = ['name', 'code', 'slug', 'status', 'actions'];

  readonly filteredInstitutions = computed(() =>
    this.institutions().filter((x) => x.name.toLowerCase().includes(this.search().toLowerCase())),
  );

  readonly pagedInstitutions = computed(() => {
    const start = this.pageIndex() * this.pageSize();

    const end = start + this.pageSize();

    return this.filteredInstitutions().slice(start, end);
  });

  onPageChange(event: PageEvent): void {
    this.pageIndex.set(event.pageIndex);

    this.pageSize.set(event.pageSize);
  }

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.institutionService.getAll().subscribe((data) => this.institutions.set(data));
  }

  createInstitution(): void {
    this.router.navigate([this.baseRoute, 'institutions', 'new']);
  }

  openInstitution(id: string): void {
    this.router.navigate([this.baseRoute, 'institutions', id]);
  }
}
