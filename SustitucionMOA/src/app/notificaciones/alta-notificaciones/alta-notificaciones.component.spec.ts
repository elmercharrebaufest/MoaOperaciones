import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AltaNotificacionesComponent } from './alta-notificaciones.component';

describe('AltaNotificacionesComponent', () => {
  let component: AltaNotificacionesComponent;
  let fixture: ComponentFixture<AltaNotificacionesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AltaNotificacionesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AltaNotificacionesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
