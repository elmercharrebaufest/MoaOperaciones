import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TabProveedoresComponent } from './tab-proveedores.component';

describe('TabProveedoresComponent', () => {
  let component: TabProveedoresComponent;
  let fixture: ComponentFixture<TabProveedoresComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TabProveedoresComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TabProveedoresComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
