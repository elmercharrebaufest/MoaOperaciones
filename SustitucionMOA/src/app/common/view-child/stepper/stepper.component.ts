import { Component, Input } from '@angular/core';
import { MenuItem } from 'primeng/api';
import { Paso } from '../../models/paso';

@Component({
    selector: 'stepper',
    templateUrl: `stepper.component.html`
})

export class StepperComponent {

    @Input() pasos:Paso[];
    items: MenuItem[];
    activeIndex: number = 3;
    readonly: boolean = false;

    constructor() {
        this.items = new Array;
    }

    ngOnInit(){
    }

    ngAfterViewInit(): void {
        if(this.pasos){
            this.pasos.forEach((p,i) => {
                var item = {
                    label: p.Nombre,
                    command: (event: any) => {
                        this.activeIndex = event.index;
                    }
                }
    
                this.items.push(item);

                this.activeIndex = p.Activo ? i : this.activeIndex;
            });
        }
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
}