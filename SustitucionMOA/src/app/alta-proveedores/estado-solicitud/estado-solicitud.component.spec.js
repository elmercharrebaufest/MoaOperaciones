import { async, TestBed } from '@angular/core/testing';
import { EstadoSolicitudComponent } from './estado-solicitud.component';
describe('EstadoSolicitudComponent', function () {
    var component;
    var fixture;
    beforeEach(async(function () {
        TestBed.configureTestingModule({
            declarations: [EstadoSolicitudComponent]
        })
            .compileComponents();
    }));
    beforeEach(function () {
        fixture = TestBed.createComponent(EstadoSolicitudComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });
    it('should create', function () {
        expect(component).toBeTruthy();
    });
});
//# sourceMappingURL=estado-solicitud.component.spec.js.map