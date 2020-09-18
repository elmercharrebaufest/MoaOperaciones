var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
import { Component, Input } from '@angular/core';
var CartaPorteModalComponent = /** @class */ (function () {
    function CartaPorteModalComponent() {
    }
    CartaPorteModalComponent.prototype.ngOnInit = function () {
        console.log(this.cartaPortID);
    };
    __decorate([
        Input(),
        __metadata("design:type", String)
    ], CartaPorteModalComponent.prototype, "cartaPortID", void 0);
    CartaPorteModalComponent = __decorate([
        Component({
            selector: 'app-carta-porte-modal',
            templateUrl: './carta-porte-modal.component.html',
            styleUrls: ['./carta-porte-modal.component.css']
        }),
        __metadata("design:paramtypes", [])
    ], CartaPorteModalComponent);
    return CartaPorteModalComponent;
}());
export { CartaPorteModalComponent };
//# sourceMappingURL=carta-porte-modal.component.js.map