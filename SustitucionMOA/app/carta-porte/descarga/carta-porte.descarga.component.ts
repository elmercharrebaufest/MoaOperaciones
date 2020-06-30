import { Component, OnInit, ViewChild } from '@angular/core';
import { CartaPorteBaseComponent } from './../carta-porte.component';
import { CartaPorteService, CartaPorteDescargaService } from './../carta-porte.service';
import { SessionDataService } from './../../common/services/SessionDataService';
import { SecurityService } from './../../common/services/SecurityService';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { ModalService } from './../../common/services/ModalService';

@Component({
    selector: 'my-app',
    templateUrl: `./app/carta-porte/descarga/carta-porte.descarga.component.html?v=${new Date().getTime()}`,
    providers: [{ provide: CartaPorteService, useClass: CartaPorteDescargaService }]
})
export class CartaPorteDescargaComponent extends CartaPorteBaseComponent {

    tituloArchivo = "ReporteDescargas.xls";
    tituloZip = "FotosCartaPorte.zip";
 


    constructor(protected service: CartaPorteService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }

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
            result => {
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

                    if (window.navigator.msSaveOrOpenBlob) {
                        // IE11
                        window.navigator.msSaveOrOpenBlob(blob, this.tituloZip);
                    } else {
                        var url = window.URL.createObjectURL(blob);
                        var link = document.createElement("a");
                        document.body.appendChild(link);
                        link.href = url;
                        link.download = this.tituloZip;
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

    checkAll(ev:any) {
        this.data.cartasPorte.forEach((x: { state: any; }) => x.state = ev.target.checked)
    }

    isAllChecked() {
        return this.data.cartasPorte.every((_: { state: any; }) => _.state);
    }
}

