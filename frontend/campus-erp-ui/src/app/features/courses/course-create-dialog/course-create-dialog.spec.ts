import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CourseCreateDialog } from './course-create-dialog';

describe('CourseCreate', () => {
  let component: CourseCreateDialog;
  let fixture: ComponentFixture<CourseCreateDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CourseCreateDialog],
    }).compileComponents();

    fixture = TestBed.createComponent(CourseCreateDialog);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
