import { Component, EventEmitter, Input, OnInit, Output } from "@angular/core";
import { Solp } from "../../../solp";

@Component({
    selector: 'app-racional-compras-solp',
    templateUrl: './racional-compras-solp.component.html',
    styleUrls: ['./racional-compras-solp.component.css']
})
export class RacionalComprasSolpComponent implements OnInit {

    @Input()
    mostrarModalRacionales: boolean = false;

    @Input()
    solp!: Solp;

    @Output()
    guardarRacionalComprasEmitter = new EventEmitter();
    
    @Output()
    cancelarRacionalComprasEmitter = new EventEmitter();
    
    condicionesDeEntrega: string = "";
    condicionesDePago: string = "";
    garantias: string = "";
    textoDeCabecera: string = "";

    mensajeError: string = "";
    tabsActivos: boolean[] = [false, false, false, false];

    ngOnInit() {
        setTimeout(() => {
            this.condicionesDeEntrega = this.solp.racional_CondicionesDeEntrega;
            this.condicionesDePago = this.solp.racional_CondicionesDePago;
            this.garantias = this.solp.racional_Garantias;
            this.textoDeCabecera = this.solp.racional_TextoDeCabecera;
        }, 2000);
    }

    onGuardar() {
        this.mensajeError = "";
        if (this.validar()) {
            this.solp.racional_CondicionesDeEntrega = this.condicionesDeEntrega;
            this.solp.racional_CondicionesDePago = this.condicionesDePago;
            this.solp.racional_Garantias = this.garantias;
            this.solp.racional_TextoDeCabecera = this.textoDeCabecera;
            this.mostrarModalRacionales = false;
            this.cerrarAccordion();
            this.guardarRacionalComprasEmitter.next();
        }
    }

    onCancelar() {
        this.mostrarModalRacionales = false;
        this.cancelarRacionalComprasEmitter.next();
        this.cerrarAccordion();
    }

    validar(): boolean {
        if (this.textoDeCabecera.length < 2) {
            this.mensajeError = "Debe ingresar texto de cabecera";
            return false;
        }
        if (this.condicionesDePago.length < 2) {
            this.mensajeError = "Debe ingresar condiciones de pago";
            return false;
        }
        return true;
    }

    cerrarAccordion() {
        this.tabsActivos = [false, false, false, false];
    }
}