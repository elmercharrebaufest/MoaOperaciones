var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
import { Pipe } from '@angular/core';
var CustomFilter = /** @class */ (function () {
    function CustomFilter() {
    }
    CustomFilter.prototype.transform = function (values, field, filter) {
        if (!values || !values.length)
            return [];
        if (!filter)
            return values;
        //Filtro compuesto
        if (filter.indexOf('|') > 0) {
            var filtros_1 = filter.split('|');
            return values.filter(function (v) { return filtros_1.some(function (f) { return v[field].toUpperCase().indexOf(f.toUpperCase()) >= 0; }); });
        }
        //Filtro simple
        return values.filter(function (v) { return v[field].toUpperCase().indexOf(filter.toUpperCase()) >= 0; });
    };
    CustomFilter = __decorate([
        Pipe({
            name: 'customFilter'
        })
    ], CustomFilter);
    return CustomFilter;
}());
export { CustomFilter };
//# sourceMappingURL=customFilter.js.map