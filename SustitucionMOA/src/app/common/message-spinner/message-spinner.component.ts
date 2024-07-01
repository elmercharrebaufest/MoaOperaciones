import { Component, OnInit } from '@angular/core';

@Component({
    selector: 'message-spinner',
    templateUrl: 'message-spinner.component.html',
    styleUrls: ['message-spinner.component.css']
})

export class MessageSpinnerComponent implements OnInit {
    private show: boolean = true;
    
    constructor() { 
    }

    ngOnInit() { 
    }

    get spinnerStatus(): boolean {
        return this.show;
    }

    public showIt(): void {
        this.show = true;
    }

    public hideIt(): void {
        this.show = false;
    }
}