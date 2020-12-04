import { async, TestBed } from '@angular/core/testing';
import { VendedoresPendientesComponent } from './vendedores-pendientes.component';
describe('VendedoresPendientesComponent', function () {
    var component;
    var fixture;
    beforeEach(async(function () {
        TestBed.configureTestingModule({
            declarations: [VendedoresPendientesComponent]
        })
            .compileComponents();
    }));
    beforeEach(function () {
        fixture = TestBed.createComponent(VendedoresPendientesComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });
    it('should create', function () {
        expect(component).toBeTruthy();
    });
});
//# sourceMappingURL=vendedores-pendientes.component.spec.js.map