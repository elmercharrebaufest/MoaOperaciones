import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TabDatosPosicionComponent } from './tab-datos-posicion.component';

describe('TabDatosPosicionComponent', () => {
  let component: TabDatosPosicionComponent;
  let fixture: ComponentFixture<TabDatosPosicionComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TabDatosPosicionComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TabDatosPosicionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
