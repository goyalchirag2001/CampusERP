import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TeacherEditDialog } from './teacher-edit-dialog';

describe('TeacherEditDialog', () => {
  let component: TeacherEditDialog;
  let fixture: ComponentFixture<TeacherEditDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TeacherEditDialog],
    }).compileComponents();

    fixture = TestBed.createComponent(TeacherEditDialog);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
