import { Component, Input, OnInit } from "@angular/core";
import { BaseComponent } from "../../../common/base-components/base-component";
import { ComprasService } from "../../compras.service";
import { NavService } from "../../../common/services/NavService";
import { SessionDataService } from "../../../common/services/SessionDataService";
import { SecurityService } from "../../../common/services/SecurityService";
import { FloatMsgService } from "../../../common/services/FloatMsgService";
import { ModalService } from "../../../common/services/ModalService";


@Component({
    selector: 'app-desvincular-po-multiple',
    templateUrl: './desvincular-po-multiple.component.html'
})
export class DesvincularPoMultiple extends BaseComponent implements OnInit {

    constructor(protected service: ComprasService, protected navService: NavService, protected sessionDataService: SessionDataService,
        protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(navService, securityService, floatMsgService, modalService);
    }

    @Input() displayDesvincularPoMultiple: boolean = false;
    @Input() solpPosicionId: number;
    @Input() listaPO: POADesvincular[] = [];

    mensajeError: string = '';

    ngOnInit() {}

    cancelarDesvinculacion() {
        this.mensajeError = '';
        this.displayDesvincularPoMultiple = false;
        this.listaPO = [];
    }

    guardarDesvinculacion() {
        this.mensajeError = '';
        const listaPOsSeleccionadas = this.listaPO.filter(val => { return val.seleccionada; }).map(x => { return x.nroPO; });
        this.service.desvincularSolpDePOMultiple(this.solpPosicionId, listaPOsSeleccionadas)
            .subscribe(
                (result) => {
                    if (result.logout) {
                        this.sessionDataService.logout();
                    }
                    else {
                        if (result.error) {
                            this.mensajeError = result.error;
                        }
                        else {
                            if (result.info) {
                                this.mensajeError = result.info;
                            }
                            else {
                                this.displayDesvincularPoMultiple = false;
                                this.listaPO = [];
                            }
                        }
                    }
                },
                (error) => {
                    this.mensajeError = error.message;
                }
            );
    }

    onSeleccionarPO(peticion: POADesvincular, event: Event) {
        const peticionesSinSeleccionar = this.listaPO.filter(
            val => {
                return !val.seleccionada;
            });

        if (peticionesSinSeleccionar.length === 1 && peticionesSinSeleccionar[0].nroPO === peticion.nroPO) {
            event.preventDefault();
        }
    }
}

export interface POADesvincular {
    nroPO: string;
    seleccionada: boolean;
}