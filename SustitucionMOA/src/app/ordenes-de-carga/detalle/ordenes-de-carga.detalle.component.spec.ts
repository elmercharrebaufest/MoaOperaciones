import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OrdenesDeCargaDetalleComponent } from './ordenes-de-carga.detalle.component';

describe('OrdenesDeCargaDetalleComponent', () => {
  let component: OrdenesDeCargaDetalleComponent;
  let fixture: ComponentFixture<OrdenesDeCargaDetalleComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OrdenesDeCargaDetalleComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OrdenesDeCargaDetalleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
