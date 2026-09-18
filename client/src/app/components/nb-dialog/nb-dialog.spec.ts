import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NbDialog } from './nb-dialog';

describe('NbDialog', () => {
  let component: NbDialog;
  let fixture: ComponentFixture<NbDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NbDialog],
    }).compileComponents();

    fixture = TestBed.createComponent(NbDialog);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
