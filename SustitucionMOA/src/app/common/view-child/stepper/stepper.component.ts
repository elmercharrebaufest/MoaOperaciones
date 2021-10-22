import { Component, EventEmitter, Input, Output } from '@angular/core';
import { MenuItem } from 'primeng/api';
import { Paso } from '../../models/paso';

@Component({
    selector: 'stepper',
    templateUrl: `stepper.component.html`
})

export class StepperComponent {

    @Input() pasos:Paso[];
    @Output() change = new EventEmitter<Paso>();

    items: MenuItem[];
    activeIndex: number = 0;
    readonly: boolean = false;
    activeStep: Paso;

    constructor() {
        this.items = new Array;
    }

    @Input() 
    set paso(value: Paso) {
        console.log("set pasos 1: ", this.pasos)
        this.activeStep = value;
        this.activeIndex = value.Numero - 1;
        this.change.emit(value)
     }
     
     get paso(): Paso {
         return this.activeStep;
     }

    ngOnInit(){
    }

    ngAfterViewInit(): void {
        console.log("Stepper mas duro que el piti", this.pasos)
        if(this.pasos){
            this.pasos.forEach((p,i) => {
                var item = {
                    label: p.Nombre
                }
    
                this.items.push(item);

                this.activeIndex = p.Activo ? i : this.activeIndex;
            });
        }
        console.log("Stepper mas duro que el piti 2", this.pasos)
    }

    getItemClass(index){
        var step = this.pasos[index];

        if(this.activeIndex == index){
            var ret = "active";

            if(index + 1 < this.pasos.length && this.pasos[index+1].Iniciado){
                ret += " " + (step.Completo ? "active-complete": "active-incomplete");
            }

            return ret;
        }
        if(step.Iniciado && step.Completo)
            return "complete";
        if(step.Iniciado && !step.Completo)
            return "incomplete";

        return "";
    }

    itemClick(event, item, index){
        console.log("Stepper 1:", this.pasos)
        var step = this.pasos[index];
        console.log("Stepper 2:", this.pasos)
        this.activeIndex = index;
        console.log("Stepper 3:", this.pasos)
        this.paso = step;
        console.log("Stepper 4:", this.pasos)
        this.change.emit(step)
        console.log("Stepper 5:", this.pasos)
    }
}