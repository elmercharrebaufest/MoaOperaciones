import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AltaProveedorComponent } from './alta-proveedor.component';

describe('AltaProveedorComponent', () => {
  let component: AltaProveedorComponent;
  let fixture: ComponentFixture<AltaProveedorComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AltaProveedorComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AltaProveedorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
