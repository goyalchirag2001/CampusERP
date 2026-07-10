import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AcademicSessionFormDialog } from './academic-session-form-dialog';

describe('AcademicSessionFormDialog', () => {
  let component: AcademicSessionFormDialog;
  let fixture: ComponentFixture<AcademicSessionFormDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AcademicSessionFormDialog],
    }).compileComponents();

    fixture = TestBed.createComponent(AcademicSessionFormDialog);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
