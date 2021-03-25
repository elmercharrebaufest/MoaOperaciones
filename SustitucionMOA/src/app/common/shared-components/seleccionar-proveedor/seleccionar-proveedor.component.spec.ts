import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SeleccionarProveedorComponent } from './seleccionar-proveedor.component';

describe('SeleccionarProveedorComponent', () => {
  let component: SeleccionarProveedorComponent;
  let fixture: ComponentFixture<SeleccionarProveedorComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SeleccionarProveedorComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SeleccionarProveedorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
