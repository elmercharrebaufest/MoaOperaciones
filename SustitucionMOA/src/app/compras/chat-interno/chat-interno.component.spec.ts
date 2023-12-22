import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ChatInternoComponent } from './chat-interno.component';

describe('ChatInternoComponent', () => {
  let component: ChatInternoComponent;
  let fixture: ComponentFixture<ChatInternoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ChatInternoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ChatInternoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
