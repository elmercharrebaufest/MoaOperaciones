import { Component, Input } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Component({
    selector: 'spinner',
    templateUrl: `spinner.component.html`
})

export class SpinnerComponent {

    @Input()visible = new BehaviorSubject<boolean>(false);

    showIt() {
        this.visible.next(true);
    }

    hideIt() {
        this.visible.next(false);
    }

}