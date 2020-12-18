import { async, TestBed } from '@angular/core/testing';
import { CarouselNotificacionesComponent } from './carousel-notificaciones.component';
describe('CarouselNotificacionesComponent', function () {
    var component;
    var fixture;
    beforeEach(async(function () {
        TestBed.configureTestingModule({
            declarations: [CarouselNotificacionesComponent]
        })
            .compileComponents();
    }));
    beforeEach(function () {
        fixture = TestBed.createComponent(CarouselNotificacionesComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });
    it('should create', function () {
        expect(component).toBeTruthy();
    });
});
//# sourceMappingURL=carousel-notificaciones.component.spec.js.map