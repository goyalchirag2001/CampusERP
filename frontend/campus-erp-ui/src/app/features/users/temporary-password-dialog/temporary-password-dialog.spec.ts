import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TemporaryPasswordDialog } from './temporary-password-dialog';

describe('TemporaryPasswordDialog', () => {
  let component: TemporaryPasswordDialog;
  let fixture: ComponentFixture<TemporaryPasswordDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TemporaryPasswordDialog],
    }).compileComponents();

    fixture = TestBed.createComponent(TemporaryPasswordDialog);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
