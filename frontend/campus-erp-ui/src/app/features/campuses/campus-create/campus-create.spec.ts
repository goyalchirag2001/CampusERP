import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CampusCreate } from './campus-create';

describe('CampusCreate', () => {
  let component: CampusCreate;
  let fixture: ComponentFixture<CampusCreate>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CampusCreate],
    }).compileComponents();

    fixture = TestBed.createComponent(CampusCreate);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
