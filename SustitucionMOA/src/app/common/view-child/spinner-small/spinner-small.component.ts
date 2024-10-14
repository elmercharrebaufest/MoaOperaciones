import { Component } from '@angular/core';

@Component({
    selector: 'spinner-small',
    templateUrl: `spinner-small.component.html`
})

export class SpinnerSmallComponent {
    visible = false;

    showIt() {
        this.visible = true;
    }

    hideIt() {
        this.visible = false;
    }
}