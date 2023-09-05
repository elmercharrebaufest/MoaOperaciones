import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CerrarCotizacionComponent } from './cerrar-cotizacion.component';

describe('CerrarCotizacionComponent', () => {
  let component: CerrarCotizacionComponent;
  let fixture: ComponentFixture<CerrarCotizacionComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CerrarCotizacionComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CerrarCotizacionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
