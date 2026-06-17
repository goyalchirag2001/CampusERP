import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InstitutionCreate } from './institution-create';

describe('InstitutionCreate', () => {
  let component: InstitutionCreate;
  let fixture: ComponentFixture<InstitutionCreate>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InstitutionCreate],
    }).compileComponents();

    fixture = TestBed.createComponent(InstitutionCreate);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
