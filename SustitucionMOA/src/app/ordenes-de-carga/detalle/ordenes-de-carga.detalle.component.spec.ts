import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OrdenesDeCarga.DetalleComponent } from './ordenes-de-carga.detalle.component';

describe('OrdenesDeCarga.DetalleComponent', () => {
  let component: OrdenesDeCarga.DetalleComponent;
  let fixture: ComponentFixture<OrdenesDeCarga.DetalleComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OrdenesDeCarga.DetalleComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OrdenesDeCarga.DetalleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
