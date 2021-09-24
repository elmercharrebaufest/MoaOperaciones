import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OrdenesDeCargaListado } from './ordenes-de-carga.listado.component';

describe('OrdenesDeCarga.ListadoComponent', () => {
  let component: OrdenesDeCargaListado;
  let fixture: ComponentFixture<OrdenesDeCargaListado>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OrdenesDeCargaListado ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OrdenesDeCargaListado);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
