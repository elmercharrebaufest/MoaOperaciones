import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TicketPesadaComponent } from './ticket-pesada.component';

describe('TicketPesadaComponent', () => {
  let component: TicketPesadaComponent;
  let fixture: ComponentFixture<TicketPesadaComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TicketPesadaComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TicketPesadaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
