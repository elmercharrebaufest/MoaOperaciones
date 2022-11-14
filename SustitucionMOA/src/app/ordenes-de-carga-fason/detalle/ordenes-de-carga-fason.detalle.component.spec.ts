import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OrdenesDeCargaFasonDetalleComponent } from './ordenes-de-carga-fason.detalle.component';

describe('DetalleComponent', () => {
    let component: OrdenesDeCargaFasonDetalleComponent;
    let fixture: ComponentFixture<OrdenesDeCargaFasonDetalleComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
        declarations: [ OrdenesDeCargaFasonDetalleComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
      fixture = TestBed.createComponent(OrdenesDeCargaFasonDetalleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
