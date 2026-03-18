import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChatNavItem } from './chat-nav-item';

describe('ChatNavItem', () => {
  let component: ChatNavItem;
  let fixture: ComponentFixture<ChatNavItem>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ChatNavItem],
    }).compileComponents();

    fixture = TestBed.createComponent(ChatNavItem);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
