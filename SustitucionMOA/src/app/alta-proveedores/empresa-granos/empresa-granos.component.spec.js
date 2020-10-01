import { async, TestBed } from '@angular/core/testing';
import { EmpresaGranosComponent } from './empresa-granos.component';
describe('EmpresaGranosComponent', function () {
    var component;
    var fixture;
    beforeEach(async(function () {
        TestBed.configureTestingModule({
            declarations: [EmpresaGranosComponent]
        })
            .compileComponents();
    }));
    beforeEach(function () {
        fixture = TestBed.createComponent(EmpresaGranosComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });
    it('should create', function () {
        expect(component).toBeTruthy();
    });
});
//# sourceMappingURL=empresa-granos.component.spec.js.map