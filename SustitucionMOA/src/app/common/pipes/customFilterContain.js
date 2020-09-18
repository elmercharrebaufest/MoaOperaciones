var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
import { Pipe } from '@angular/core';
var CustomFilterContain = /** @class */ (function () {
    function CustomFilterContain() {
    }
    CustomFilterContain.prototype.transform = function (values, field, innerField, filterValue) {
        if (!values || !values.length)
            return [];
        if (!filterValue)
            return values;
        var filtered = [];
        for (var i = 0; i < values.length; i++) {
            for (var j = 0; j < values[i][field].length; j++) {
                if (values[i][field][j][innerField].toUpperCase().indexOf(filterValue.toUpperCase()) >= 0) {
                    filtered.push(values[i]);
                    break;
                }
            }
        }
        return filtered;
    };
    CustomFilterContain = __decorate([
        Pipe({
            name: 'customFilterContain'
        })
    ], CustomFilterContain);
    return CustomFilterContain;
}());
export { CustomFilterContain };
//# sourceMappingURL=customFilterContain.js.map