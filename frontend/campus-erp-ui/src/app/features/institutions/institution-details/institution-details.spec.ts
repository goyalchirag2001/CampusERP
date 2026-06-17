import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InstitutionDetails } from './institution-details';

describe('InstitutionDetails', () => {
  let component: InstitutionDetails;
  let fixture: ComponentFixture<InstitutionDetails>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InstitutionDetails],
    }).compileComponents();

    fixture = TestBed.createComponent(InstitutionDetails);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
