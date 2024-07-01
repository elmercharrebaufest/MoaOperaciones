import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ModalAltaEntradaDeServicioProveedorComponent } from './modal-alta-entrada-de-servicio-proveedor.component';

describe('ModalAltaEntradaDeServicioProveedorComponent', () => {
  let component: ModalAltaEntradaDeServicioProveedorComponent;
  let fixture: ComponentFixture<ModalAltaEntradaDeServicioProveedorComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ModalAltaEntradaDeServicioProveedorComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ModalAltaEntradaDeServicioProveedorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
