import { Component, Input, OnInit, inject, signal, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDialog } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';

import { Semester } from '../../semesters/models/semester';
import { Subject } from '../../subjects/models/subject';
import { SemesterSubject } from '../../semester-subjects/models/semester-subject';
import { SemesterSubjectService } from '../../semester-subjects/services/semester-subject';
import { NotificationService } from '../../../core/services/notification';

import { LookupPickerComponent } from '../../../shared/components/lookup-picker/lookup-picker';
import { LookupPickerItem } from '../../../shared/components/lookup-picker/lookup-picker.model';

@Component({
  selector: 'app-semester-card',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatFormFieldModule, MatInputModule, MatIconModule],
  templateUrl: './semester-card.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrl: './semester-card.scss',
})
export class SemesterCard implements OnInit {
  private readonly dialog = inject(MatDialog);

  private readonly semesterSubjectService = inject(SemesterSubjectService);

  private readonly notificationService = inject(NotificationService);

  @Input({ required: true })
  semester!: Semester;

  @Input({ required: true })
  subjects: Subject[] = [];

  readonly assignedSubjects = signal<SemesterSubject[]>([]);

  ngOnInit(): void {
    this.load();
  }

  private load(): void {
    this.semesterSubjectService.getBySemester(this.semester.id).subscribe((data) => {
      data.sort((a, b) => a.displayOrder - b.displayOrder);

      this.assignedSubjects.set(data);
    });
  }

  selectAndAssignSubject(): void {
    const assignedIds = this.assignedSubjects().map((x) => x.subjectId);

    const items: LookupPickerItem[] = this.subjects
      .filter((x) => !assignedIds.includes(x.id))
      .map((x) => ({
        id: x.id,
        title: x.code,
        subtitle: x.name,
        tag: `${x.credits} Credits`,
        payload: x,
      }));

    this.dialog
      .open(LookupPickerComponent, {
        width: '700px',
        maxWidth: '95vw',
        maxHeight: '90vh',
        data: {
          title: `Assign Subject - ${this.semester.name}`,
          placeholder: 'Search subject code or name',
          items,
        },
      })
      .afterClosed()
      .subscribe((item: LookupPickerItem | undefined) => {
        if (!item) {
          return;
        }

        this.semesterSubjectService
          .assign({
            semesterId: this.semester.id,
            subjectId: item.id,
          })
          .subscribe({
            next: () => {
              this.notificationService.success('Subject assigned successfully.');

              this.load();
            },

            error: (err) => {
              this.notificationService.error(err.error?.message ?? 'Unable to assign subject.');
            },
          });
      });
  }

  remove(id: string): void {
    this.semesterSubjectService.remove(id).subscribe(() => {
      this.notificationService.success('Subject removed.');

      this.load();
    });
  }

  moveUp(id: string): void {
    this.semesterSubjectService.moveUp(id).subscribe(() => this.load());
  }

  moveDown(id: string): void {
    this.semesterSubjectService.moveDown(id).subscribe(() => this.load());
  }
}
