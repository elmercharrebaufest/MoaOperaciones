import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ObtenerContratoMarcoComponent } from './obtener-contrato-marco.component';

describe('ObtenerContratoMarcoComponent', () => {
  let component: ObtenerContratoMarcoComponent;
  let fixture: ComponentFixture<ObtenerContratoMarcoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ObtenerContratoMarcoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ObtenerContratoMarcoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
