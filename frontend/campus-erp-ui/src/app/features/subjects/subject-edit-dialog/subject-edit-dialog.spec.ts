import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SubjectEditDialog } from './subject-edit-dialog';

describe('SubjectEditDialog', () => {
  let component: SubjectEditDialog;
  let fixture: ComponentFixture<SubjectEditDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SubjectEditDialog],
    }).compileComponents();

    fixture = TestBed.createComponent(SubjectEditDialog);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
