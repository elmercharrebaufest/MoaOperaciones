import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ListadoNovedadesComponent } from './listado-novedades.component';

describe('ListadoNovedadesComponent', () => {
  let component: ListadoNovedadesComponent;
  let fixture: ComponentFixture<ListadoNovedadesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ListadoNovedadesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ListadoNovedadesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
