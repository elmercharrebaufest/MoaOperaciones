import { async, TestBed } from '@angular/core/testing';
import { CuitInvalidoComponent } from './cuit-invalido.component';
describe('CuitInvalidoComponent', function () {
    var component;
    var fixture;
    beforeEach(async(function () {
        TestBed.configureTestingModule({
            declarations: [CuitInvalidoComponent]
        })
            .compileComponents();
    }));
    beforeEach(function () {
        fixture = TestBed.createComponent(CuitInvalidoComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });
    it('should create', function () {
        expect(component).toBeTruthy();
    });
});
//# sourceMappingURL=cuit-invalido.component.spec.js.map