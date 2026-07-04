import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CourseEditDialog } from './course-edit-dialog';

describe('CourseEditDialog', () => {
  let component: CourseEditDialog;
  let fixture: ComponentFixture<CourseEditDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CourseEditDialog],
    }).compileComponents();

    fixture = TestBed.createComponent(CourseEditDialog);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
