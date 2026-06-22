import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DepartmentCreateDialog } from './department-create-dialog';

describe('DepartmentCreate', () => {
  let component: DepartmentCreateDialog;
  let fixture: ComponentFixture<DepartmentCreateDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DepartmentCreateDialog],
    }).compileComponents();

    fixture = TestBed.createComponent(DepartmentCreateDialog);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
