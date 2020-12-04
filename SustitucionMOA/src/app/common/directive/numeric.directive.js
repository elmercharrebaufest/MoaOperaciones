var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
import { Directive, ElementRef, HostListener, Input } from '@angular/core';
var NumericDirective = /** @class */ (function () {
    function NumericDirective(el) {
        this.el = el;
        this.decimals = 0;
        this.specialKeys = [
            'Backspace', 'Tab', 'End', 'Home', 'ArrowLeft', 'ArrowRight', 'Delete'
        ];
    }
    NumericDirective.prototype.check = function (value, decimals) {
        if (decimals <= 0) {
            return String(value).match(new RegExp(/^\d+$/));
        }
        else {
            var regExpString = "^\\s*((\\d+(\\.\\d{0," + decimals + "})?)|((\\d*(\\.\\d{1," + decimals + "}))))\\s*$";
            return String(value).match(new RegExp(regExpString));
        }
    };
    NumericDirective.prototype.onKeyDown = function (event) {
        if (this.specialKeys.indexOf(event.key) !== -1) {
            return;
        }
        // Do not use event.keycode this is deprecated.
        // See: https://developer.mozilla.org/en-US/docs/Web/API/KeyboardEvent/keyCode
        var current = this.el.nativeElement.value;
        var next = current.concat(event.key);
        if (next && !this.check(next, this.decimals)) {
            event.preventDefault();
        }
    };
    __decorate([
        Input('decimals'),
        __metadata("design:type", Number)
    ], NumericDirective.prototype, "decimals", void 0);
    __decorate([
        HostListener('keydown', ['$event']),
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [KeyboardEvent]),
        __metadata("design:returntype", void 0)
    ], NumericDirective.prototype, "onKeyDown", null);
    NumericDirective = __decorate([
        Directive({
            selector: '[numeric]'
        }),
        __metadata("design:paramtypes", [ElementRef])
    ], NumericDirective);
    return NumericDirective;
}());
export { NumericDirective };
//# sourceMappingURL=numeric.directive.js.map