import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CarouselNotificacionesComponent } from './carousel-notificaciones.component';

describe('CarouselNotificacionesComponent', () => {
  let component: CarouselNotificacionesComponent;
  let fixture: ComponentFixture<CarouselNotificacionesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CarouselNotificacionesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CarouselNotificacionesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
