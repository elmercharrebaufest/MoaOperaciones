import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CotizacionHistorialComponent } from './cotizacion-historial.component';

describe('CotizacionHistorialComponent', () => {
  let component: CotizacionHistorialComponent;
  let fixture: ComponentFixture<CotizacionHistorialComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CotizacionHistorialComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CotizacionHistorialComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
