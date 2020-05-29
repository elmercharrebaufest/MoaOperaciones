"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var CustomFilter = /** @class */ (function () {
    function CustomFilter() {
    }
    CustomFilter.prototype.transform = function (values, field, filter) {
        if (!values || !values.length)
            return [];
        if (!filter)
            return values;
        return values.filter(function (v) { return v[field].toUpperCase().indexOf(filter.toUpperCase()) >= 0; });
    };
    CustomFilter = __decorate([
        core_1.Pipe({
            name: 'customFilter'
        })
    ], CustomFilter);
    return CustomFilter;
}());
exports.CustomFilter = CustomFilter;
//# sourceMappingURL=customFilter.js.map