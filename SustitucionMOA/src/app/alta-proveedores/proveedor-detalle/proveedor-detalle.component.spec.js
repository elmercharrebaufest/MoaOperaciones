import { async, TestBed } from '@angular/core/testing';
import { ProveedorDetalleComponent } from './proveedor-detalle.component';
describe('ProveedorDetalleComponent', function () {
    var component;
    var fixture;
    beforeEach(async(function () {
        TestBed.configureTestingModule({
            declarations: [ProveedorDetalleComponent]
        })
            .compileComponents();
    }));
    beforeEach(function () {
        fixture = TestBed.createComponent(ProveedorDetalleComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });
    it('should create', function () {
        expect(component).toBeTruthy();
    });
});
//# sourceMappingURL=proveedor-detalle.component.spec.js.map