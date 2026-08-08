import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TimetableTemplateFormDialog } from './timetable-template-form-dialog';

describe('TimetableTemplateFormDialog', () => {
  let component: TimetableTemplateFormDialog;
  let fixture: ComponentFixture<TimetableTemplateFormDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TimetableTemplateFormDialog],
    }).compileComponents();

    fixture = TestBed.createComponent(TimetableTemplateFormDialog);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
