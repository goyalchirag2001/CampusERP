import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TimetableTemplateDetails } from './timetable-template-details';

describe('TimetableTemplateDetails', () => {
  let component: TimetableTemplateDetails;
  let fixture: ComponentFixture<TimetableTemplateDetails>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TimetableTemplateDetails],
    }).compileComponents();

    fixture = TestBed.createComponent(TimetableTemplateDetails);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
