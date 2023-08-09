import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OrdenesDeCargaFasonListadoComponent } from './ordenes-de-carga-fason.listado.component';

describe('ListadoComponent', () => {
    let component: OrdenesDeCargaFasonListadoComponent;
    let fixture: ComponentFixture<OrdenesDeCargaFasonListadoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
        declarations: [ OrdenesDeCargaFasonListadoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
      fixture = TestBed.createComponent(OrdenesDeCargaFasonListadoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
