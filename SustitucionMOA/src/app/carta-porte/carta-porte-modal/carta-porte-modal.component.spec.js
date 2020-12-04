import { async, TestBed } from '@angular/core/testing';
import { CartaPorteModalComponent } from './carta-porte-modal.component';
describe('CartaPorteModalComponent', function () {
    var component;
    var fixture;
    beforeEach(async(function () {
        TestBed.configureTestingModule({
            declarations: [CartaPorteModalComponent]
        })
            .compileComponents();
    }));
    beforeEach(function () {
        fixture = TestBed.createComponent(CartaPorteModalComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });
    it('should create', function () {
        expect(component).toBeTruthy();
    });
});
//# sourceMappingURL=carta-porte-modal.component.spec.js.map