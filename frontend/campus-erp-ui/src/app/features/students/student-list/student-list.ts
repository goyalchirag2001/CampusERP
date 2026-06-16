import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';

import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { FormsModule } from '@angular/forms';

import { StudentService } from '../../../core/services/student';
import { StudentResponse } from '../../../core/models/student-response';
import { StudentDrawer } from '../student-drawer/student-drawer';

@Component({
  selector: 'app-student-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    StudentDrawer,
  ],
  templateUrl: './student-list.html',
  styleUrl: './student-list.scss',
})
export class StudentList implements OnInit {
  private readonly studentService = inject(StudentService);

  students: StudentResponse[] = [];

  filteredStudents: StudentResponse[] = [];

  searchText = '';

  ngOnInit(): void {
    this.loadStudents();
  }

  loadStudents(): void {
    this.studentService.getAll().subscribe({
      next: (students) => {
        this.students = students;

        this.filteredStudents = students;
      },
    });
  }

  onSearch(): void {
    const search = this.searchText.toLowerCase();

    this.filteredStudents = this.students.filter(
      (x) =>
        x.firstName.toLowerCase().includes(search) ||
        x.lastName.toLowerCase().includes(search) ||
        x.email.toLowerCase().includes(search) ||
        x.rollNumber.toLowerCase().includes(search),
    );
  }

  isDrawerOpen = false;

  openDrawer(): void {
    this.isDrawerOpen = true;
  }

  closeDrawer(): void {
    this.isDrawerOpen = false;
  }

  onStudentSaved(): void {
    this.loadStudents();

    this.closeDrawer();
  }
}
