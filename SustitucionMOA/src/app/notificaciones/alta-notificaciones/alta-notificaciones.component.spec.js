import { async, TestBed } from '@angular/core/testing';
import { AltaNotificacionesComponent } from './alta-notificaciones.component';
describe('AltaNotificacionesComponent', function () {
    var component;
    var fixture;
    beforeEach(async(function () {
        TestBed.configureTestingModule({
            declarations: [AltaNotificacionesComponent]
        })
            .compileComponents();
    }));
    beforeEach(function () {
        fixture = TestBed.createComponent(AltaNotificacionesComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });
    it('should create', function () {
        expect(component).toBeTruthy();
    });
});
//# sourceMappingURL=alta-notificaciones.component.spec.js.map