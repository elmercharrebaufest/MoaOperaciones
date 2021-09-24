import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OrdenesDeCargaAlta } from './ordenes-de-carga.alta.component';

describe('OrdenesDeCarga.AltaComponent', () => {
  let component: OrdenesDeCargaAlta;
  let fixture: ComponentFixture<OrdenesDeCargaAlta>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OrdenesDeCargaAlta ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OrdenesDeCargaAlta);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
