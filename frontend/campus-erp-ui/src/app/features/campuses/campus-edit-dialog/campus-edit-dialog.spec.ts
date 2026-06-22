import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CampusEditDialog } from './campus-edit-dialog';

describe('CampusEditDialog', () => {
  let component: CampusEditDialog;
  let fixture: ComponentFixture<CampusEditDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CampusEditDialog],
    }).compileComponents();

    fixture = TestBed.createComponent(CampusEditDialog);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
