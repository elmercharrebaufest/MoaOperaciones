import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ConsultaTicketPesadaComponent } from './consulta-ticket-pesada.component';

describe('ConsultaTicketPesadaComponent', () => {
  let component: ConsultaTicketPesadaComponent;
  let fixture: ComponentFixture<ConsultaTicketPesadaComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ConsultaTicketPesadaComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ConsultaTicketPesadaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
