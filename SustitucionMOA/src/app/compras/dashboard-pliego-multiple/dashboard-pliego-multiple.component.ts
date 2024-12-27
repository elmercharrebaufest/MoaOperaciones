import { Component, OnInit } from '@angular/core';

@Component({
    selector: 'app-dashboard-pliego-multiple',
    templateUrl: './dashboard-pliego-multiple.component.html',
    styleUrls: ['./dashboard-pliego-multiple.component.css']
})
export class DashboardPliegoMultipleComponent implements OnInit {

    public nombrePliego?: string = null;

    constructor() { }

    ngOnInit() {
    }

}
