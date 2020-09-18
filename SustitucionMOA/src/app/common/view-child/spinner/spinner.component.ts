import { Component } from '@angular/core';

@Component({
    selector: 'spinner',
    templateUrl: `spinner.component.html`
})

export class SpinnerComponent {

    visible = false;

    showIt() {
        this.visible = true;
    }

    hideIt() {
        this.visible = false;
    }

}