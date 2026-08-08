import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TimetableTemplateList } from './timetable-template-list';

describe('TimetableTemplateList', () => {
  let component: TimetableTemplateList;
  let fixture: ComponentFixture<TimetableTemplateList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TimetableTemplateList],
    }).compileComponents();

    fixture = TestBed.createComponent(TimetableTemplateList);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
