import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { VendedoresPendientesComponent } from './vendedores-pendientes.component';

describe('VendedoresPendientesComponent', () => {
  let component: VendedoresPendientesComponent;
  let fixture: ComponentFixture<VendedoresPendientesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ VendedoresPendientesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(VendedoresPendientesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
