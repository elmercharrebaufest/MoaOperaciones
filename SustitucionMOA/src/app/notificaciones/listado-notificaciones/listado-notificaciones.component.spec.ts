import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ListadoNotificacionesComponent } from './listado-notificaciones.component';

describe('ListadoNotificacionesComponent', () => {
  let component: ListadoNotificacionesComponent;
  let fixture: ComponentFixture<ListadoNotificacionesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ListadoNotificacionesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ListadoNotificacionesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
