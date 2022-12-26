import { Component, Output, EventEmitter, Input, OnInit, OnChanges } from '@angular/core';

export class DropdownOption {
    value: string;
    label: string;

    constructor(value: string, label: string) {
        this.value = value;
        this.label = label;
    }
}
export type TipoDropdown = "numberItems" | "selectInput"|"periodos"|"filtroVariable"|"pageInput"

@Component({
    selector: 'dropdown',
    templateUrl: `dropdown.component.html`
})

export class DropdownComponent implements OnInit {

    @Input() tipoDropdown: TipoDropdown;

    @Input()
    options: Array<DropdownOption>;

    @Output()
    select = new EventEmitter();

    selectedOption: string;
    selectedOptionLabel: string = "";

    constructor() {
        this.select = new EventEmitter();
    }

    ngOnInit() {
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
        };
    }

    setItemsPerPageValues() {
        this.setOptions([
            new DropdownOption("10", "10"),
            new DropdownOption("20", "20"),
            new DropdownOption("50", "50"),
            new DropdownOption("100", "100")
        ]);
        this.setSelectItem(sessionStorage.getItem("itemsPerPage") ? sessionStorage.getItem("itemsPerPage") : "10");
    }

    setPeriodos() {
        this.setOptions([
            new DropdownOption("1", "Últimos dos dias"),
            new DropdownOption("2", "Última semana"),
            new DropdownOption("3", "Último mes"),
            new DropdownOption("5", "Últimos dos meses"),
            new DropdownOption("4", "Entre Fechas")
        ]);
    }

    setPageInputOptions() {
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
    }

    setInitial() {
        this.setOptions([
            new DropdownOption("", "Todos")
        ]);
        this.setSelectItem("");
    }

    setSelectInputOptions() {
        this.setOptions([
            new DropdownOption("", "Cargando...")
        ]);
        this.setSelectItem("");

    }

    setOptions(options: Array<DropdownOption>) {
        this.options = options 
    }

    getSelectedLabel() {
        return this.selectedOptionLabel;
    }

    selectItem(value: string, label: any) {
        this.selectedOptionLabel = label;
        this.selectedOptionLabel = "";
        this.setSelectItem(value);
        this.select.emit(value);
    }

    setSelectItem(value: string) {
        this.selectedOption = value;
        if (this.options != null) {
            for (let option of this.options) {
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
        };
    }
}