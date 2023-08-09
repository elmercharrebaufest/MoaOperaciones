import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OrdenesDeCargaFasonAltaComponent } from './ordenes-de-carga-fason.alta.component';

describe('AltaComponent', () => {
    let component: OrdenesDeCargaFasonAltaComponent;
    let fixture: ComponentFixture<OrdenesDeCargaFasonAltaComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
        declarations: [ OrdenesDeCargaFasonAltaComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OrdenesDeCargaFasonAltaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
