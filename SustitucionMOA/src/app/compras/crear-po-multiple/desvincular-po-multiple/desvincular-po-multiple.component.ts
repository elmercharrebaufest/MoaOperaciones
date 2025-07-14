import { Component, EventEmitter, Input, OnInit, Output } from "@angular/core";
import { BaseComponent } from "../../../common/base-components/base-component";
import { ComprasService } from "../../compras.service";
import { NavService } from "../../../common/services/NavService";
import { SessionDataService } from "../../../common/services/SessionDataService";
import { SecurityService } from "../../../common/services/SecurityService";
import { FloatMsgService } from "../../../common/services/FloatMsgService";
import { ModalService } from "../../../common/services/ModalService";
import { PeticionDeOfertaDesvincularDto } from "../../../modelos/compras/POMultiple/peticionDeOfertaDesvincularDto";
import { BlockUI, NgBlockUI } from "ng-block-ui";


@Component({
    selector: 'app-desvincular-po-multiple',
    templateUrl: './desvincular-po-multiple.component.html'
})
export class DesvincularPoMultiple extends BaseComponent implements OnInit {

    constructor(protected service: ComprasService, protected navService: NavService, protected sessionDataService: SessionDataService,
        protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(navService, securityService, floatMsgService, modalService);
    }

    //@BlockUI() blockUI: NgBlockUI;
    @BlockUI() blockUI: NgBlockUI;

    @Input() displayDesvincularPoMultiple: boolean = false;
    @Input() idPosicionOSolp: number; // Contiene el id de la SOLP (para Servicio) o de la solpPosicion (para Material) a desvincular
    @Input() peticiones: PeticionDeOfertaDesvincularDto[] = [];
    @Input() esMaterial: boolean;

    @Output() cerrarDesvinculacionEmitter = new EventEmitter();
    @Output() guardarDesvinculacionEmitter = new EventEmitter();

    mensajeError: string = '';

    ngOnInit() {}

    cancelarDesvinculacion() {
        this.mensajeError = '';
        this.displayDesvincularPoMultiple = false;
        this.peticiones = [];
        this.cerrarDesvinculacionEmitter.next();
    }

    guardarDesvinculacion() {
        this.blockUI.start();
        this.mensajeError = '';
        const listaPOsSeleccionadas = this.peticiones.filter(pet => { return pet.Seleccionada; }).map(x => { return x.NroPeticion.toString(); });

        let desvincularObservable = this.esMaterial ?
            this.service.desvincularSolpDePOMultipleMaterial(this.idPosicionOSolp, listaPOsSeleccionadas) :
            this.service.desvincularSolpDePOMultipleServicio(this.idPosicionOSolp, listaPOsSeleccionadas);

        desvincularObservable.subscribe(
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
                                this.peticiones = [];
                                this.guardarDesvinculacionEmitter.next();
                            }
                        }
                    }
                    this.blockUI.stop();
                },
                (error) => {
                    this.mensajeError = error.message;
                    this.blockUI.stop();
                }
            );
    }

    hayPOSeleccionada(): boolean {
        return this.peticiones.filter(pet => { return pet.Seleccionada; }).length > 0;
    }
}
