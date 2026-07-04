import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SubjectDetails } from './subject-details';

describe('SubjectDetails', () => {
  let component: SubjectDetails;
  let fixture: ComponentFixture<SubjectDetails>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SubjectDetails],
    }).compileComponents();

    fixture = TestBed.createComponent(SubjectDetails);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
