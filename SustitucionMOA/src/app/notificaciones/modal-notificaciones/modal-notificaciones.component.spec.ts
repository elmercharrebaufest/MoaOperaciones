import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ModalNotificacionesComponent } from './modal-notificaciones.component';

describe('ModalNotificacionesComponent', () => {
  let component: ModalNotificacionesComponent;
  let fixture: ComponentFixture<ModalNotificacionesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ModalNotificacionesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ModalNotificacionesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
