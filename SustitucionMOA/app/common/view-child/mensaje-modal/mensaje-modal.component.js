"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var MensajeModalComponent = /** @class */ (function () {
    function MensajeModalComponent() {
        this.errorMsg = "";
        this.successMsg = "";
    }
    MensajeModalComponent.prototype.setMsgsEmpty = function () {
        this.errorMsg = "";
        this.successMsg = "";
    };
    MensajeModalComponent.prototype.setErrorMsg = function (msg) {
        this.errorMsg = msg;
    };
    MensajeModalComponent.prototype.setSuccessMsg = function (msg) {
        this.successMsg = msg;
    };
    MensajeModalComponent = __decorate([
        core_1.Component({
            selector: 'mensaje-modal',
            templateUrl: "./app/common/view-child/mensaje-modal/mensaje-modal.component.html?v=" + new Date().getTime()
        })
    ], MensajeModalComponent);
    return MensajeModalComponent;
}());
exports.MensajeModalComponent = MensajeModalComponent;
//# sourceMappingURL=mensaje-modal.component.js.map