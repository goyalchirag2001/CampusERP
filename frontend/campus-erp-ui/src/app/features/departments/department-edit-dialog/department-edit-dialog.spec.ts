import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DepartmentEditDialog } from './department-edit-dialog';

describe('DepartmentEditDialog', () => {
  let component: DepartmentEditDialog;
  let fixture: ComponentFixture<DepartmentEditDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DepartmentEditDialog],
    }).compileComponents();

    fixture = TestBed.createComponent(DepartmentEditDialog);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
