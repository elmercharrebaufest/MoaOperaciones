import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { ComprasService } from '../compras.service';
import { MensajeComponent } from '../../common/view-child/mensaje/mensaje.component';
import { LegajoExternoDto } from '../../modelos/legajoExternoDto';

@Component({
    selector: 'app-legajo-externo',
    templateUrl: './legajo-externo.component.html',
    styleUrls: ['./../dashboard-comprador/legajo/legajo.component.css', '../compras.component.css']
})
export class LegajoExternoComponent implements OnInit {
    @BlockUI() blockUI: NgBlockUI;

    protected mensajeComponent: MensajeComponent;
    legajo: LegajoExternoDto;
    error: string;
    visualizarAlert = false;
    adjudicacionId: string;
    token: string;
    cargarPantalla = false;

    constructor(private route: ActivatedRoute, protected service: ComprasService) {
    }

    ngOnInit() {
        this.route.params.subscribe((params) => {
            this.adjudicacionId = params['id'];
            this.token = params['token'];
        });

        this.verLegajo();
    }

    verLegajo() {
        this.blockUI.start('Cargando...');
        this.service.verLegajoParaExternos(this.adjudicacionId, this.token)
            .subscribe(
                (result) => {
                    this.legajo = result.data;
                    this.blockUI.stop();
                    this.cargarPantalla = true;
                },
                (error) => {
                    this.blockUI.stop();
                    this.mensajeComponent.setErrorMsg(error.message);
                });
    }

    descargarArchivo(archivoId: number, tipoLegajo) {
        if (tipoLegajo == "SOLP") {
            let SolpId = this.legajo.ListaLegajos[0].SolpId;
            this.blockUI.start("Generando...");
            this.service.getPdf(SolpId)
                .subscribe(
                    (result) => {
                        var byteArray = new Uint8Array(result.FileContents);
                        var blob = new Blob([byteArray], {
                            type: "application/octet-stream",
                        });

                        this.downloadArchivoLocal(blob, result.FileDownloadName);
                        this.blockUI.stop();
                    },
                    (error) => {
                        this.blockUI.stop();
                        this.mensajeComponent.setErrorMsg(error.message);
                    })
        } else if (tipoLegajo == "Petición de Oferta") {
            this.blockUI.start("Generando...");
            this.service.getPdfPeticionDeOfertaUsuario(archivoId)
                .subscribe(
                    (result) => {
                        var byteArray = new Uint8Array(result.FileContents);
                        var blob = new Blob([byteArray], {
                            type: "application/octet-stream",
                        });

                        this.downloadArchivoLocal(blob, result.FileDownloadName);
                        this.blockUI.stop();
                    },
                    (error) => {
                        this.blockUI.stop();
                        this.mensajeComponent.setErrorMsg(error.message);
                    })
        } else if (tipoLegajo == "Chat Interno") {
            let SolpId = this.legajo.ListaLegajos[0].SolpId;
            this.blockUI.start("Generando...");
            this.service.obtenerYExportarChat(SolpId.toString())
                .subscribe(
                    (result) => {
                        var byteArray = new Uint8Array(result.FileContents);
                        var blob = new Blob([byteArray], {
                            type: "text/plain",
                        });

                        this.downloadArchivoLocal(blob, result.FileDownloadName);
                        this.blockUI.stop();
                    },
                    (error) => {
                        this.blockUI.stop();
                        this.mensajeComponent.setErrorMsg(error.message);
                    })
        } else {
            this.blockUI.start("Descargando...");
            this.service.DescargarArchivo(archivoId)
                .subscribe(
                    (result) => {
                        var byteArray = new Uint8Array(result.FileContents);
                        var blob = new Blob([byteArray], {
                            type: "application/octet-stream",
                        });
                        this.downloadArchivoLocal(blob, result.FileDownloadName);
                        this.blockUI.stop();
                    },
                    (error) => {
                        this.mensajeComponent.setErrorMsg(error.message);
                        this.blockUI.stop();
                    }
                )
        }
    }

    private downloadArchivoLocal(blob: Blob, nombreArchivo: string): void {
        if (window.navigator.msSaveOrOpenBlob) {
            window.navigator.msSaveOrOpenBlob(
                blob,
                nombreArchivo
            );
        } else {
            var url = window.URL.createObjectURL(blob);
            var link = document.createElement("a");
            document.body.appendChild(link);
            link.href = url;
            link.download = nombreArchivo;
            link.click();
            setTimeout(function () {
                window.URL.revokeObjectURL(url);
            }, 0);
            return;
        }
    }

    descargarLegajo() {
        let idPeticion = this.legajo.ListaLegajos[0].PeticionDeOfertaId;
        this.blockUI.start('Generando...');
        this.service.descargarLegajo(idPeticion, null)
            .subscribe(
                (result) => {
                    var byteArray = new Uint8Array(result.FileContents);
                    var blob = new Blob([byteArray], {
                        type: "application/octet-stream",
                    });

                    if (window.navigator.msSaveOrOpenBlob) {
                        window.navigator.msSaveOrOpenBlob(
                            blob,
                            result.FileDownloadName
                        );
                    } else {
                        var url = window.URL.createObjectURL(blob);
                        var link = document.createElement("a");
                        document.body.appendChild(link);
                        link.href = url;
                        link.download = result.FileDownloadName;
                        link.click();
                        setTimeout(function () {
                            window.URL.revokeObjectURL(url);
                        }, 0);
                        this.blockUI.stop();
                        return false;
                    }
                    this.blockUI.stop();
                },
                (error) => {
                    this.mensajeComponent.setErrorMsg(error.message);
                    this.blockUI.stop();
                })
    }
}