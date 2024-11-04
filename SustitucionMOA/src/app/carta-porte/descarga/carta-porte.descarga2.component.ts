import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { CartaPorteBaseComponent } from './../carta-porte.component';
import { CartaPorteService, CartaPorteDescargaService } from './../carta-porte2.service';
import { SessionDataService } from './../../common/services/SessionDataService';
import { SecurityService } from './../../common/services/SecurityService';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { ModalService } from './../../common/services/ModalService';
import baseParse from 'base64-arraybuffer';
import { ActivatedRoute } from '@angular/router';
import { SendDataService } from '../../consulta/send-data.service';
import { TipoPeriodo } from '../../common/enums/TipoPeriodo';

declare var Tiff: any;


@Component({
    selector: 'app-carta-porte-descarga',
    templateUrl: `carta-porte.descarga.component.html`,
    providers: [{ provide: CartaPorteService, useClass: CartaPorteDescargaService }]
})
export class CartaPorteDescargaComponent extends CartaPorteBaseComponent {
    tituloArchivo = "ReporteDescargas.xls";
    tituloZip = "FotosCartaPorte.zip";

    constructor(protected service: CartaPorteService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService, protected route: ActivatedRoute, protected sendDataService: SendDataService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService, route);
    }

    cartaPorteId = "";
    fotoSrc = "";
    cartaPorteDescarga = "";
    showModalBox = false;
    data: any;
    filtroFechaKey: string = 'GCPDesc_Periodo'
    filtroFechaPeriodoDefault: TipoPeriodo = TipoPeriodo.UltimaSemana;

    checkPermisos() { this.securityService.tienePermisoRedirect("CONSULTAR CARTAS PORTE"); }

    setTabs() {
        this.setMenuSeccionTab("carta-porte", "Descargas");
    }

    showModalTableResponsive(recepcionInfo: any) {
        this.modalService.openModalTableResponsive("Carta de Porte", [
            { etiqueta: "Fecha", valor: recepcionInfo.fechaDescarga },
            { etiqueta: "CCPP Nº", valor: recepcionInfo.cartaPorte },
            { etiqueta: "Producto", valor: recepcionInfo.producto },
            { etiqueta: "Contrato", valor: recepcionInfo.contrnum },
            { etiqueta: "CG", valor: recepcionInfo.cg },
            { etiqueta: "Recibido", valor: recepcionInfo.netoDescontadoString },
            { etiqueta: "Vendedor", valor: recepcionInfo.vendedor }
        ]);
        return false;
    }

    validarCheckboxesFotos() {
        let cartaPorteIDStr = "";

        cartaPorteIDStr = this.selectedOptions();

        if (cartaPorteIDStr == "") {
            this.mensajeComponent.setMsgsEmpty();
            this.mensajeComponent.setInfoMsg("Debe seleccionar las cartas de porte que quiere descargar");
        }
        else {
            this.descargarFotos(cartaPorteIDStr);
        }
    }

    selectedOptions() {
        if (!this.data)
            return "";

        return this.data.cartasPorte
            .filter(function (e: { state: boolean; }) { return e.state })
            .map(function (e: { cartaPorte: string; }) { return e.cartaPorte })
            .join(",");
    }

    descargarFotos(cartaPorteIDStr: string) {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerSmallComponent.showIt();
        this.unsubscribe();
        this.subscription = this.service.descargarFotos(cartaPorteIDStr).subscribe(
            (result: any) => {
                this.spinnerSmallComponent.hideIt();
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {
                    var byteArray = new Uint8Array(result.FileContents);
                    var blob = new Blob([byteArray], { type: 'application/zip' });
                    var url = window.URL.createObjectURL(blob);
                    var link = document.createElement("a");
                    document.body.appendChild(link);
                    link.href = url;
                    link.download = this.tituloZip;
                    link.click();
                    setTimeout(function () { window.URL.revokeObjectURL(url); }, 0);
                    return false;
                }
            },
            error => {
                this.spinnerSmallComponent.hideIt();
                this.mensajeComponent.setErrorMsg(error.message);
            }
        );
        return false;  // <- Prevent href del a
    }

    abrirModal(cartaDePorteNumero: string) {
        this.spinnerSmallComponent.showIt();
        this.floatMsgService.setMsgsEmpty();
        this.unsubscribe();
        this.subscription = this.service.getFotos(cartaDePorteNumero).subscribe(

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
                    this.cartaPorteDescarga = cartaDePorteNumero;
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

    checkAll(ev: any) {
        this.data.cartasPorte.forEach((x: { state: any; }) => x.state = ev.target.checked)
    }

    isAllChecked() {
        return this.data.cartasPorte.every((_: { state: any; }) => _.state);
    }

    loadFileTiff(imagen: any): string {
        let imganenBuffer = baseParse.decode(imagen); //convierto el byte[] aun array buffer
        let archivoTiff = new Tiff({ buffer: imganenBuffer });
        return archivoTiff.toDataURL(); //obtiene un texto plano png del archivo a mostrar
    }
    consultaDisconformidadCalidad(informacion) {
        this.sendDataService.setDatosDisconformidadCalidades({
            NroCCPP: informacion.cartaPorte,
            NroContrato: informacion.contrnum,
            Material: informacion.producto
        })
        return this.goToSeccionParam("consulta", "crear-consulta")
    }
    navegarAConsultaCalidad(informacion) {
        this.sendDataService.setDatosCartaPorteConDisconformidadCalidades({
            NroCCPP: informacion.cartaPorte,
            NroContrato: informacion.contrnum,
        })
        return this.goToSeccionParam("consulta", "mis-consultas")
    }
}

