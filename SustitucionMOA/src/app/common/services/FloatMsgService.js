var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
var FloatMsgService = /** @class */ (function () {
    function FloatMsgService() {
        this.errorMsj = new Subject();
        this.infoMsj = new Subject();
        this.successMsj = new Subject();
        this.errorMsj$ = this.errorMsj.asObservable();
        this.infoMsj$ = this.infoMsj.asObservable();
        this.successMsj$ = this.successMsj.asObservable();
    }
    FloatMsgService.prototype.setMsgsEmpty = function () {
        this.errorMsj.next("");
    };
    FloatMsgService.prototype.setErrorMsg = function (value) {
        this.errorMsj.next(value);
    };
    FloatMsgService.prototype.setInfoMsg = function (value) {
        this.infoMsj.next(value);
    };
    FloatMsgService.prototype.setSuccessMsg = function (value) {
        this.successMsj.next(value);
    };
    FloatMsgService = __decorate([
        Injectable(),
        __metadata("design:paramtypes", [])
    ], FloatMsgService);
    return FloatMsgService;
}());
export { FloatMsgService };
//# sourceMappingURL=FloatMsgService.js.map