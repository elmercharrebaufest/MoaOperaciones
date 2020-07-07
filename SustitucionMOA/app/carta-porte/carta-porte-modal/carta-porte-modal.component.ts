import { Component, OnInit, Input } from '@angular/core';

@Component({
    selector: 'app-carta-porte-modal',
    templateUrl: './carta-porte-modal.component.html',
    styleUrls: ['./carta-porte-modal.component.css']
})
export class CartaPorteModalComponent implements OnInit {
    @Input()
    cartaPortID: string;
    constructor() { }

    ngOnInit(): void {

        console.log(this.cartaPortID);

    }

}
