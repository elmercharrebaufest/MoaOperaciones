import { async, TestBed } from '@angular/core/testing';
import { EmpresaNoGranosComponent } from './empresa-no-granos.component';
describe('EmpresaNoGranosComponent', function () {
    var component;
    var fixture;
    beforeEach(async(function () {
        TestBed.configureTestingModule({
            declarations: [EmpresaNoGranosComponent]
        })
            .compileComponents();
    }));
    beforeEach(function () {
        fixture = TestBed.createComponent(EmpresaNoGranosComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });
    it('should create', function () {
        expect(component).toBeTruthy();
    });
});
//# sourceMappingURL=empresa-no-granos.component.spec.js.map