import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { CartaPorteService } from './../carta-porte2.service';
import { MensajeComponent } from './../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../../common/view-child/spinner/spinner.component';
import { SpinnerSmallComponent } from './../../common/view-child/spinner-small/spinner-small.component';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { SecurityService } from './../../common/services/SecurityService';
import { Seccion } from './../../common/models/seccion';
import { BaseComponent } from './../../common/base-components/base-component';
import { SessionDataService } from './../../common/services/SessionDataService';
import { ModalService } from './../../common/services/ModalService';
import baseParse from 'base64-arraybuffer';
declare var Tiff: any;


@Component({
    selector: 'app-carta-porte-detalle',
    templateUrl: `carta-porte.detalle.component.html`,
    providers: [CartaPorteService]
})
export class CartaPorteDetalleComponent extends BaseComponent implements OnInit, AfterViewInit {

    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    @ViewChild("smallSpinner")
    protected spinnerSmallComponent: SpinnerSmallComponent;


    constructor(protected service: CartaPorteService, protected navService: NavService, private route: ActivatedRoute, private router: Router, protected securityService: SecurityService, protected sessionDataService: SessionDataService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(navService, securityService, floatMsgService, modalService);
        this.mensajeComponent = new MensajeComponent();
        this.spinnerComponent = new SpinnerComponent();
    }

    data: any;
    cartaPorteId = "";
    tituloArchivo = "ReporteCartaPorteDetalle.xls";
    fotoSrc = "";
    showModalBox = false;
    condicionCamara: any;
    condicionCalada: any;
    kgDescuentoString: any;
    porcentajeDescuento: any;

    setTabs() {
        this.setMenuSeccionTab("carta-porte", "Detalle");
    }

    ngOnInit() {
        this.setTabs();
        this.securityService.tienePermisoRedirect("CONSULTAR CARTAS PORTE DETALLE");
        var secciones = [new Seccion('/carta-porte/descarga', 'carta-porte', 'Descargas'), new Seccion('/carta-porte/aplicacion', 'carta-porte', 'Aplicaciones')];
        if (this.isAuthorized("CREAR FORMULARIO CCPP"))
            secciones.push(new Seccion('/carta-porte/formulario', 'carta-porte', 'Formulario'));
        this.navService.setSeccionList(secciones);
        this.getData();
    }

    ngAfterViewInit() {
        this.spinnerSmallComponent = new SpinnerSmallComponent();
    }

    getData() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.route.params.forEach((params: Params) => {
            this.cartaPorteId = params['id'];
            this.unsubscribe();
            this.subscription = this.service.getDetalle(this.cartaPorteId).subscribe(
                (result: any) => {
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.data = result.data;
                        console.log("Data", this.data)
                        const calidadesACamara = result.data.datosCalidad.filter(cal=>!cal.caracteristica.toUpperCase().includes("HUMEDAD"))
                        this.condicionCamara = calidadesACamara
                                .every(x => x.resultadoCamara == 0);
                        this.porcentajeDescuento = calidadesACamara
                                .every(x => x.porcentajeDescuento == 0);
                    }
                },
                error => {
                    this.spinnerComponent.hideIt();
                    this.mensajeComponent.setErrorMsg(error.message);
                }

            );
        });
    }

    exportExcel() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerSmallComponent.showIt();
        this.unsubscribe();
        this.subscription = this.service.exportExcelDetalle(this.cartaPorteId).subscribe(
            (result: any) => {
                this.spinnerSmallComponent.hideIt();
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {
                    var blob = new Blob([result], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
                    if (window.navigator.msSaveOrOpenBlob) {
                        // IE11
                        window.navigator.msSaveOrOpenBlob(blob, this.tituloArchivo);
                    } else {
                        var url = window.URL.createObjectURL(blob);
                        var link = document.createElement("a");
                        document.body.appendChild(link);
                        link.href = url;
                        link.download = this.tituloArchivo;
                        link.click();
                        setTimeout(function () { window.URL.revokeObjectURL(url); }, 0);
                        return false;
                    }
                }
            },
            error => {
                this.spinnerSmallComponent.hideIt();
                this.mensajeComponent.setErrorMsg(error.message);
            }
        );
        return false;  // <- Prevent href del a
    }

    descargarPDF() {
        this.floatMsgService.setMsgsEmpty();
        this.unsubscribe();
        this.subscription = this.service.exportPDFCalidad(this.cartaPorteId).subscribe(
            (result: any) => {
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.floatMsgService.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.floatMsgService.setInfoMsg(result.info);
                } else {
                    var byteArray = new Uint8Array(result.data);
                    var blob = new Blob([byteArray], { type: 'application/pdf' });
                    if (window.navigator.msSaveOrOpenBlob) {
                        // IE11
                        window.navigator.msSaveOrOpenBlob(blob, "Calidad Carta de Porte(" + this.cartaPorteId + ").pdf");
                    } else {
                        var url = window.URL.createObjectURL(blob);
                        var link = document.createElement("a");
                        document.body.appendChild(link);
                        link.href = url;
                        link.download = "Calidad Carta de Porte(" + this.cartaPorteId + ").pdf"
                        link.click();
                        setTimeout(function () { window.URL.revokeObjectURL(url); }, 0);
                        return false;
                    }
                }
            },
            error => {
                this.floatMsgService.setErrorMsg(error.message);
            }
        );
        return false;
    }

    isExportVisible() {
        this.spinnerSmallComponent.visible;
    }

    showDataPlus(calidad: any) {
        return this.tieneData(calidad.porcentajeDescuento);
    }

    tieneData(value: any) {
        return value != undefined && value != 0 && value != "";
    }

    abrirModal() {
        console.log("funcionHOLA")
        this.spinnerSmallComponent.showIt();
        this.floatMsgService.setMsgsEmpty();
        this.unsubscribe();
        this.subscription = this.service.getFotos(this.cartaPorteId).subscribe(
            (result: any) => {
                this.spinnerSmallComponent.hideIt();
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.floatMsgService.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.floatMsgService.setInfoMsg(result.info);
                } else {
                    if (result[0].EsArchivoTiff) {
                        this.fotoSrc = this.loadFileTiff(result[0].Foto);
                    }
                    else {
                        this.fotoSrc = 'data:image/png;base64,' + result[0].Foto;
                    }
                    document.getElementById("openModalHiddenButton").click();
                    return true;
                }
            },
            error => {
                this.floatMsgService.setErrorMsg(error.message);
            }
        );
        return false;
    }

    loadFileTiff(imagen: any): string {
        let imganenBuffer = baseParse.decode(imagen); //convierto el byte[] aun array buffer
        let archivoTiff = new Tiff({ buffer: imganenBuffer });
        return archivoTiff.toDataURL(); //obtiene un texto plano png del archivo a mostrar
    }
}
