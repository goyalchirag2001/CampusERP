import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SubjectCreateDialog } from './subject-create-dialog';

describe('SubjectCreateDialog', () => {
  let component: SubjectCreateDialog;
  let fixture: ComponentFixture<SubjectCreateDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SubjectCreateDialog],
    }).compileComponents();

    fixture = TestBed.createComponent(SubjectCreateDialog);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
