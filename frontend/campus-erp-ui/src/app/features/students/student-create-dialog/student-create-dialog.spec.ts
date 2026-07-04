import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentCreateDialog } from './student-create-dialog';

describe('StudentCreateDialog', () => {
  let component: StudentCreateDialog;
  let fixture: ComponentFixture<StudentCreateDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StudentCreateDialog],
    }).compileComponents();

    fixture = TestBed.createComponent(StudentCreateDialog);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
