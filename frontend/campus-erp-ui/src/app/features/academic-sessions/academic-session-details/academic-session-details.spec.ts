import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AcademicSessionDetails } from './academic-session-details';

describe('AcademicSessionDetails', () => {
  let component: AcademicSessionDetails;
  let fixture: ComponentFixture<AcademicSessionDetails>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AcademicSessionDetails],
    }).compileComponents();

    fixture = TestBed.createComponent(AcademicSessionDetails);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
