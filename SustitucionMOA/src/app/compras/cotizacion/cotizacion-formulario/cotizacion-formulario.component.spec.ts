import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CotizacionFormularioComponent } from './cotizacion-formulario.component';

describe('CotizacionFormularioComponent', () => {
  let component: CotizacionFormularioComponent;
  let fixture: ComponentFixture<CotizacionFormularioComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CotizacionFormularioComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CotizacionFormularioComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
