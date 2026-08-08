import { TestBed } from '@angular/core/testing';

import { TimetableTemplateService } from './timetable-template';

describe('TimetableTemplateService', () => {
  let service: TimetableTemplateService ;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(TimetableTemplateService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
