import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ModalAltaEntradaDeServicioComponent } from './modal-alta-entrada-de-servicio.component';

describe('ModalAltaEntradaDeServicioComponent', () => {
  let component: ModalAltaEntradaDeServicioComponent;
  let fixture: ComponentFixture<ModalAltaEntradaDeServicioComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ModalAltaEntradaDeServicioComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ModalAltaEntradaDeServicioComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
