var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
import { Component } from '@angular/core';
var MensajeComponent = /** @class */ (function () {
    function MensajeComponent() {
        this.errorMsg = "";
        this.infoMsg = "";
        this.successMsg = "";
    }
    MensajeComponent.prototype.setMsgsEmpty = function () {
        this.errorMsg = "";
        this.infoMsg = "";
        this.successMsg = "";
    };
    MensajeComponent.prototype.setErrorMsg = function (msg) {
        window.scroll(0, 0);
        this.errorMsg = msg;
    };
    MensajeComponent.prototype.setInfoMsg = function (msg) {
        window.scroll(0, 0);
        this.infoMsg = msg;
    };
    MensajeComponent.prototype.setSuccessMsg = function (msg) {
        window.scroll(0, 0);
        this.successMsg = msg;
    };
    MensajeComponent = __decorate([
        Component({
            selector: 'mensaje',
            templateUrl: "mensaje.component.html"
        })
    ], MensajeComponent);
    return MensajeComponent;
}());
export { MensajeComponent };
//# sourceMappingURL=mensaje.component.js.map