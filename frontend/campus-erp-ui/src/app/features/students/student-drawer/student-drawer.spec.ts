import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentDrawer } from './student-drawer';

describe('StudentDrawer', () => {
  let component: StudentDrawer;
  let fixture: ComponentFixture<StudentDrawer>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StudentDrawer],
    }).compileComponents();

    fixture = TestBed.createComponent(StudentDrawer);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
