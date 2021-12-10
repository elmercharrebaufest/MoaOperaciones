import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TabDireccionEntregaComponent } from './tab-direccion-entrega.component';

describe('TabDireccionEntregaComponent', () => {
  let component: TabDireccionEntregaComponent;
  let fixture: ComponentFixture<TabDireccionEntregaComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TabDireccionEntregaComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TabDireccionEntregaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
