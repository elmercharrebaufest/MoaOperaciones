import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FiltroDashboardProveedorComponent } from './filtro-dashboard-proveedor.component';

describe('FiltroDashboardProveedorComponent', () => {
  let component: FiltroDashboardProveedorComponent;
  let fixture: ComponentFixture<FiltroDashboardProveedorComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FiltroDashboardProveedorComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FiltroDashboardProveedorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
