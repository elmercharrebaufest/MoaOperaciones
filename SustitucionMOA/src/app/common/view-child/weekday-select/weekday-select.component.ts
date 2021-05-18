import { WeekDay } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { forEach } from '@angular/router/src/utils/collection';
import { WeekDayItem } from '../../models/weekDayItem';



@Component({
    selector: 'weekday-select',
    templateUrl: `weekday-select.component.html`
})

export class WeekdaySelectComponent {

    // @Output() change = new EventEmitter<Paso>();

    days: any[];
    _model: WeekDayItem[];

    @Input("model") 
    set model(value: WeekDayItem[]) {
        this._model = value;

        value.forEach(day => {
            var d = this.days.find(x => x.weekDay == day.weekDay);
            d.selected = day.selected;
        });


     }
     
     get model(): WeekDayItem[] {
        // this.days.forEach(day => {
        //     var d = this._model.find(x => x.weekDay == day.weekDay);
        //     d.selected = day.selected;
        // });
         return this._model;
     }



    constructor() {
        this.days = [
            {
                weekDay: WeekDay.Monday,
                dayName: "Lunes",
                dayNameShort: "Lun",
                dayNameMin: "L"
            },
            {
                weekDay: WeekDay.Tuesday,
                dayName: "Martes",
                dayNameShort: "Mar",
                dayNameMin: "M"
            },
            {
                weekDay: WeekDay.Wednesday,
                dayName: "Miercoles",
                dayNameShort: "Mie",
                dayNameMin: "X"
            },
            {
                weekDay: WeekDay.Thursday,
                dayName: "Jueves",
                dayNameShort: "Jue",
                dayNameMin: "J"
            },
            {
                weekDay: WeekDay.Friday,
                dayName: "Viernes",
                dayNameShort: "Vie",
                dayNameMin: "V"
            },
            {
                weekDay: WeekDay.Saturday,
                dayName: "Sabado",
                dayNameShort: "Sab",
                dayNameMin: "S"
            },
            {
                weekDay: WeekDay.Sunday,
                dayName: "Domingo",
                dayNameShort: "Dom",
                dayNameMin: "D"
            }
        ]

    }

    handleChange(event, weekDay){
        var d = this._model.find(x => x.weekDay == weekDay);
        d.selected = event.checked;
    }


    ngOnInit(){
    }

    ngAfterViewInit(): void {

    }

}