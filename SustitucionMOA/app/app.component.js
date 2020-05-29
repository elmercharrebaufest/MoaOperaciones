"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var router_1 = require("@angular/router");
var ServiceLocator_1 = require("./common/services/ServiceLocator");
var AppComponent = /** @class */ (function () {
    function AppComponent(injector, router) {
        this.injector = injector;
        this.router = router;
        ServiceLocator_1.ServiceLocator.injector = this.injector;
        this.router.events.subscribe(function (event) {
            if (event instanceof router_1.NavigationEnd) {
                try {
                    ga('set', 'page', event.urlAfterRedirects);
                    ga('send', 'pageview');
                }
                catch (e) { }
            }
        });
    }
    AppComponent = __decorate([
        core_1.Component({
            selector: 'my-app',
            templateUrl: "./app/app.component.html"
        }),
        __metadata("design:paramtypes", [core_1.Injector, router_1.Router])
    ], AppComponent);
    return AppComponent;
}());
exports.AppComponent = AppComponent;
//# sourceMappingURL=app.component.js.map