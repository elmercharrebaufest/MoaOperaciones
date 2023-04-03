import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ListadoDashboardProveedorComponent } from './listado-dashboard-proveedor.component';

describe('ListadoDashboardProveedorComponent', () => {
  let component: ListadoDashboardProveedorComponent;
  let fixture: ComponentFixture<ListadoDashboardProveedorComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ListadoDashboardProveedorComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ListadoDashboardProveedorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
