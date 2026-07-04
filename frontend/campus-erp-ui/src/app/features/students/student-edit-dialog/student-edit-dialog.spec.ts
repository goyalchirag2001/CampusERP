import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentEditDialog } from './student-edit-dialog';

describe('StudentEditDialog', () => {
  let component: StudentEditDialog;
  let fixture: ComponentFixture<StudentEditDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StudentEditDialog],
    }).compileComponents();

    fixture = TestBed.createComponent(StudentEditDialog);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
