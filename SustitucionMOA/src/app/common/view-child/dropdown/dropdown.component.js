var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
import { Component, Output, EventEmitter, Input } from '@angular/core';
var DropdownOption = /** @class */ (function () {
    function DropdownOption(value, label) {
        this.value = value;
        this.label = label;
    }
    return DropdownOption;
}());
export { DropdownOption };
var DropdownComponent = /** @class */ (function () {
    function DropdownComponent() {
        this.select = new EventEmitter();
        this.selectedOptionLabel = "";
        this.select = new EventEmitter();
    }
    DropdownComponent.prototype.ngOnInit = function () {
        switch (this.tipoDropdown) {
            case "numberItems":
                this.setItemsPerPageValues();
                break;
            case "periodos":
                this.setPeriodos();
                break;
            case "filtroVariable":
                this.setInitial();
                break;
            case "selectInput":
                this.setSelectInputOptions();
                break;
            case "pageInput":
                this.setPageInputOptions();
                break;
            default:
                break;
        }
        ;
    };
    DropdownComponent.prototype.setItemsPerPageValues = function () {
        this.setOptions([
            new DropdownOption("10", "10"),
            new DropdownOption("20", "20"),
            new DropdownOption("50", "50"),
            new DropdownOption("100", "100")
        ]);
        this.setSelectItem(sessionStorage.getItem("itemsPerPage") ? sessionStorage.getItem("itemsPerPage") : "10");
    };
    DropdownComponent.prototype.setPeriodos = function () {
        this.setOptions([
            new DropdownOption("1", "Últimos dos dias"),
            new DropdownOption("2", "Última semana"),
            new DropdownOption("3", "Último mes"),
            new DropdownOption("5", "Últimos dos meses"),
            new DropdownOption("4", "Entre Fechas")
        ]);
    };
    DropdownComponent.prototype.setPageInputOptions = function () {
        this.setOptions([
            new DropdownOption("1", "1"),
            new DropdownOption("5", "5"),
            new DropdownOption("9", "9"),
            new DropdownOption("13", "13"),
            new DropdownOption("17", "17"),
            new DropdownOption("21", "21"),
            new DropdownOption("25", "25"),
            new DropdownOption("29", "29"),
            new DropdownOption("33", "33"),
            new DropdownOption("37", "37"),
            new DropdownOption("41", "41"),
            new DropdownOption("45", "45"),
            new DropdownOption("49", "49"),
            new DropdownOption("53", "53"),
            new DropdownOption("57", "57"),
            new DropdownOption("61", "61"),
            new DropdownOption("65", "65"),
            new DropdownOption("69", "69"),
            new DropdownOption("73", "73"),
            new DropdownOption("77", "77"),
            new DropdownOption("81", "81"),
            new DropdownOption("85", "85"),
            new DropdownOption("89", "89"),
            new DropdownOption("93", "93"),
            new DropdownOption("97", "97")
        ]);
        this.setSelectItem("1");
    };
    DropdownComponent.prototype.setInitial = function () {
        this.setOptions([
            new DropdownOption("", "Todos")
        ]);
        this.setSelectItem("");
    };
    DropdownComponent.prototype.setSelectInputOptions = function () {
        this.setOptions([
            new DropdownOption("", "Cargando...")
        ]);
        this.setSelectItem("");
    };
    DropdownComponent.prototype.setOptions = function (options) {
        this.options = options;
    };
    DropdownComponent.prototype.getSelectedLabel = function () {
        return this.selectedOptionLabel;
    };
    DropdownComponent.prototype.selectItem = function (value, label) {
        this.selectedOptionLabel = label;
        this.selectedOptionLabel = "";
        this.setSelectItem(value);
        this.select.emit(value);
    };
    DropdownComponent.prototype.setSelectItem = function (value) {
        this.selectedOption = value;
        if (this.options != null) {
            for (var _i = 0, _a = this.options; _i < _a.length; _i++) {
                var option = _a[_i];
                if (option.value == value) {
                    this.selectedOptionLabel = option.label;
                    break;
                }
            }
        }
        switch (this.tipoDropdown) {
            case "numberItems":
                sessionStorage.setItem("itemsPerPage", this.selectedOption);
                break;
            case "periodos":
                sessionStorage.setItem("periodo", this.selectedOption);
                break;
            default:
                break;
        }
        ;
    };
    __decorate([
        Input(),
        __metadata("design:type", String)
    ], DropdownComponent.prototype, "tipoDropdown", void 0);
    __decorate([
        Input(),
        __metadata("design:type", Array)
    ], DropdownComponent.prototype, "options", void 0);
    __decorate([
        Output(),
        __metadata("design:type", Object)
    ], DropdownComponent.prototype, "select", void 0);
    DropdownComponent = __decorate([
        Component({
            selector: 'dropdown',
            templateUrl: "dropdown.component.html"
        }),
        __metadata("design:paramtypes", [])
    ], DropdownComponent);
    return DropdownComponent;
}());
export { DropdownComponent };
//# sourceMappingURL=dropdown.component.js.map