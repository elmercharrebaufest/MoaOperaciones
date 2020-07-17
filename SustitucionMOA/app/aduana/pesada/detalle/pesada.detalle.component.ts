import { Component, OnInit, ViewChild } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { AduanaService } from './../../aduana.service';
import { FiltroFechaComponent } from './../../../common/view-child/filtro-fecha/filtro-fecha.component';
import { MensajeComponent } from './../../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../../../common/view-child/spinner/spinner.component';
import { AduanaBaseComponent } from './../../aduana.component';
import { NavService } from './../../../common/services/NavService';
import { FloatMsgService } from './../../../common/services/FloatMsgService';
import { SessionDataService } from './../../../common/services/SessionDataService';
import { SecurityService } from './../../../common/services/SecurityService';
import { ModalService } from './../../../common/services/ModalService';



@Component({
    selector: 'app-aduana-pesada-detalle',
    templateUrl: `./app/aduana/pesada/detalle/pesada.detalle.component.html?v=${new Date().getTime()}`,
    providers: [AduanaService]
})

export class PesadaDetalleComponent extends AduanaBaseComponent {

    titulo = "";
    itemsPerPage = 20;
    pesadasDetalleList: any = null;
    subscription: any;

    @ViewChild(MensajeComponent)
    private mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    private spinnerComponent: SpinnerComponent;

    constructor(private route: ActivatedRoute, private router: Router, protected navService: NavService, protected service: AduanaService, protected securityService: SecurityService, protected sessionDataService: SessionDataService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(navService, service, securityService, floatMsgService, modalService);
        this.mensajeComponent = new MensajeComponent();
        this.spinnerComponent = new SpinnerComponent();
    }

    checkPermisos() { this.securityService.tienePermisoRedirect("CONSULTAR PESADA DETALLE"); }

    setTabs() {
        this.setMenuSeccionTab("aduana", "Detalle");
    }

    ngOnInit() {
        super.ngOnInit();
        this.getData();
    }

    getData() {
        this.pesadasDetalleList = null;
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.route.params.forEach((params: Params) => {
            let centro = params['centro'];
            let nroOrden = params['nroOrden'];
            this.unsubscribe();
            this.subscription = this.service.getPesadaDetalle(centro, nroOrden).subscribe(
                result => {
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.pesadasDetalleList = result;
                    }
                },
                error => {
                    this.spinnerComponent.hideIt();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
        });
    }

    isVisible() {
        return this.pesadasDetalleList != null;
    }
}