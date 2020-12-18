import { async, TestBed } from '@angular/core/testing';
import { ListadoNotificacionesComponent } from './listado-notificaciones.component';
describe('ListadoNotificacionesComponent', function () {
    var component;
    var fixture;
    beforeEach(async(function () {
        TestBed.configureTestingModule({
            declarations: [ListadoNotificacionesComponent]
        })
            .compileComponents();
    }));
    beforeEach(function () {
        fixture = TestBed.createComponent(ListadoNotificacionesComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });
    it('should create', function () {
        expect(component).toBeTruthy();
    });
});
//# sourceMappingURL=listado-notificaciones.component.spec.js.map