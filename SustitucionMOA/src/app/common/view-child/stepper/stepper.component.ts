
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { MenuItem } from 'primeng/api';
import { Paso } from '../../models/paso';

@Component({
    selector: 'stepper',
    templateUrl: `stepper.component.html`
})

export class StepperComponent {

    @Input() pasos: Paso[];
    @Output() change = new EventEmitter<Paso>();
    @Output() save = new EventEmitter();

    items: MenuItem[];
    activeIndex: number = 0;
    readonly: boolean = false;
    activeStep: Paso;

    constructor() {
        this.items = new Array;
    }

    @Input()
    set paso(value: Paso) {
        this.activeStep = value;
        this.activeIndex = value.Numero - 1;
        this.change.emit(value)
    }

    get paso(): Paso {
        return this.activeStep;
    }

    ngOnInit() {
    }

    ngAfterViewInit(): void {
        if (this.pasos) {
            this.pasos.forEach((p, i) => {
                var item = {
                    label: p.Nombre
                }

                this.items.push(item);

                this.activeIndex = p.Activo ? i : this.activeIndex;
            });
        }
    }

    getItemClass(index) {
        var step = this.pasos[index];

        if (step.Deshabilitado) {
            return "disabled";
        }

        if (this.activeIndex == index) {
            var ret = "active";

            if (index + 1 < this.pasos.length && this.pasos[index + 1].Iniciado) {
                ret += " " + (step.Completo ? "active-complete" : "active-incomplete");
            }

            return ret;
        }

        if (step.Iniciado && step.Completo)
            return "complete";
        if (step.Iniciado && !step.Completo)
            return "incomplete";

        return "";
    }

    itemClick(event, item, index) {
        var step = this.pasos[index];
        if (!step.Deshabilitado) {
            this.saveStep(step)
            this.activeIndex = index;
            this.paso = step;
            this.change.emit(step)
        }
    }

    saveStep(step: Paso) {
        if(step.Numero > this.paso.Numero) {
            this.save.emit();
        }
    }

}