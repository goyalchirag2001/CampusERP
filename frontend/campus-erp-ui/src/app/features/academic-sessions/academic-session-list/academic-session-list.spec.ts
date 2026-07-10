import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AcademicSessionList } from './academic-session-list';

describe('AcademicSessionList', () => {
  let component: AcademicSessionList;
  let fixture: ComponentFixture<AcademicSessionList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AcademicSessionList],
    }).compileComponents();

    fixture = TestBed.createComponent(AcademicSessionList);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
