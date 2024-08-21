import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AprobacionExternaService } from './aprobacion-externa.service';
import { ComprasService } from '../compras/compras.service';
import { SpinnerComponent } from '../common/view-child/spinner/spinner.component';
import { MessageSpinnerComponent } from '../common/message-spinner/message-spinner.component';

@Component({
  selector: 'app-aprobacion-externa',
  templateUrl: './aprobacion-externa.component.html',
    styleUrls: ['./aprobacion-externa.component.css'],
    providers: [AprobacionExternaService]
})

export class AprobacionExternaComponent implements OnInit {

    action: string;
    rejectionFreeText: string = "";
    rejectionCause: string = "";
    aprobacionesList: any[] = [];
    subscription: any;
    nroESLocal: string = "";
    estado: string = "";

    prov: string = "";
    //Datos Generales
    usuario: string = "";
    cert: string = "";
    certSap: string = '';
    fechaCarga: string = "";
    desc: string = "";
    importe: string = "";
    ordenCompra: string = "";
    nroPosicion: string = "";

    //TableData
    tableBody: string = "";

    //flags
    approvalSuccess: boolean = false;
    rejectionSuccess: boolean = false;
    approvalError: boolean = false;
    rejectionError: boolean = false;
    isButtonDisabled: boolean = false;
    isApprover: boolean = false;
    oldES: boolean = false;

    //Datos DtoRechazo
    detalleServicios: any[] = [];


    constructor(private route: ActivatedRoute, private router: Router, protected aprobacionExternaService: AprobacionExternaService, protected comprasService: ComprasService) {
        this.messageSpinnerComponent = new MessageSpinnerComponent();
    }

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    @ViewChild(MessageSpinnerComponent)
    protected messageSpinnerComponent: MessageSpinnerComponent;


    ngOnInit(): void {
        this.messageSpinnerComponent.showIt();
        this.route.url.subscribe(url => {
            // Extract action from URL or route parameters
            this.action = url[1].path === 'approve' ? 'approve' : 'reject';
            this.nroESLocal = url[2].path;
        });
        this.approvalSuccess = false;
        this.rejectionSuccess = false;
        this.approvalError = false;
        this.rejectionError = false;
        this.isButtonDisabled = false;
        this.getESData();
    }


    // Method to handle deselection
    deselectRadio(value: string) {
        if (this.rejectionCause === value) {
            this.rejectionCause = ''; // Deselect the radio button
        }
        else {
            this.rejectionCause = value;
        }
    }

    getESData() {
        if (this.nroESLocal !== "") {
            this.subscription = this.aprobacionExternaService.getESData(this.nroESLocal)
                .subscribe(
                    (result: any) => {
                        if (result.data.aprobador === true) {
                            this.isApprover = true;
                        }
                        if (result.data.versionAnt) {
                            this.oldES = true;
                        }
                        if (this.isApprover && !this.oldES) {
                            this.prov = result.data.proveedor;
                            this.aprobacionesList = result.data.aprobacionesList;
                            this.ordenCompra = result.data.nroOc;
                            if (this.aprobacionesList.length > 0) {
                                this.usuario = this.aprobacionesList[0].Ingresante_CDS;
                                this.cert = this.aprobacionesList[0].NRO_ES_LOCAL;
                                this.nroPosicion = this.aprobacionesList[0].NRO_POS;
                                this.estado = this.aprobacionesList[0].Estado_certificacion;
                                let dateString = this.aprobacionesList[0].Fecha_Carga_ES.toString();
                                let ts = parseInt(dateString.match(/\d+/)[0], 10);
                                let jsonDate = new Date(ts);
                                const day = ('0' + jsonDate.getDate()).slice(-2);
                                const month = ('0' + (jsonDate.getMonth() + 1)).slice(-2);
                                const year = jsonDate.getFullYear();
                                this.fechaCarga = `${day}/${month}/${year}`;
                                this.desc = this.aprobacionesList[0].Texto_breve_servicio;
                                this.aprobacionesList[0].Monto_total = this.round(this.aprobacionesList[0].Monto_total, 2);
                                this.importe = '$ ' + this.aprobacionesList[0].Monto_total.toString();
                                this.aprobacionesList.forEach(ap => {
                                    ap.Monto_a_certificar = this.round(ap.Monto_a_certificar, 2);

                                    let detServicio = {
                                        Descripcion: ap.Descripcion_ES,
                                        Cantidad: ap.Cantidad_a_certificar.toString(),
                                        UM: ap.UM,
                                        Porcentaje: ap.Porcentaje_a_certificar,
                                        Monto: "$ " + this.round(ap.Monto_a_certificar, 2).toString(),
                                    };

                                    this.detalleServicios.push(detServicio);

                                })
                            }
                            if (this.action === 'approve' && this.estado.includes('Pendiente')) {
                                this.aprobarES();
                            }
                        }
                        setTimeout(()=> {
                            this.messageSpinnerComponent.hideIt();
                        }, 5000)
                    },
                    error => {
                    }
                );
            return false; //<-- Prevent Refresh
        }

    }

    //Redondeo de decimales
    round(num: number, decimals: number) {
        return Number(num.toFixed(decimals));
    }

    //Aprobar Btn
    aprobarES() {
        try {
            this.comprasService.enviarAprobacionES(this.cert, "ARP").subscribe((resp: any) => {
                this.certSap = resp.data.NroESSap;
                this.approvalSuccess = true;
            },
                error => {
                    this.approvalError = true;
                });
        }
        catch (e) {         
            this.approvalError = true;
        }
    }

    rechazarES() {
        let rejCauseConcat = "";
        if (this.rejectionFreeText !== undefined && this.rejectionFreeText !== "") {
            rejCauseConcat = this.rejectionCause + " - " + this.rejectionFreeText;
        }
        else {
            rejCauseConcat = this.rejectionCause;
        }
        
        const data = {
            Destinatario: this.usuario,
            MotivoRechazo: rejCauseConcat,
            Proveedor: this.prov,
            NumeroCertificacion: this.cert,
            FechaCertificacion: this.fechaCarga,
            Descripcion: this.desc,
            Importe: "$ " + this.importe,
            MontoTotal: this.importe,
            DetalleServicio: this.detalleServicios,
        };

        try {
            this.spinnerComponent.showIt();
            this.isButtonDisabled = true;
            this.comprasService.enviarMotivoRechazoES(data).subscribe((resp: any) => {
                this.spinnerComponent.hideIt();
                this.rejectionSuccess = true;
            },
                error => {
                    this.spinnerComponent.hideIt();
                    this.rejectionError = true;
                });
        }
        catch (e) {
            this.spinnerComponent.hideIt();
            this.rejectionError = true;
        }

    }
}
