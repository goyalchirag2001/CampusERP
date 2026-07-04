import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TeacherCreateDialog } from './teacher-create-dialog';

describe('TeacherCreateDialog', () => {
  let component: TeacherCreateDialog;
  let fixture: ComponentFixture<TeacherCreateDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TeacherCreateDialog],
    }).compileComponents();

    fixture = TestBed.createComponent(TeacherCreateDialog);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
