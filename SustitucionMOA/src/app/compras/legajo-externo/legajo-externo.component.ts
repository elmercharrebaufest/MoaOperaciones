import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { ComprasService } from '../compras.service';
import { MensajeComponent } from '../../common/view-child/mensaje/mensaje.component';
import { LegajoExternoDto } from '../../modelos/legajoExternoDto';
import { LegajoDto, LegajoTipo } from '../../modelos/compras/legajoDto';

@Component({
    selector: 'app-legajo-externo',
    templateUrl: './legajo-externo.component.html',
    styleUrls: ['./../legajo/legajo.component.css', '../compras.component.css']
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
    previousId: number | null = null;
    toggleColor: boolean = false;

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

    descargarArchivo(legajoDto: LegajoDto) {
        let SolpId = legajoDto.SolpId;
        if (legajoDto.Tipo == "SOLP") {
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
        } else if (legajoDto.Tipo == "Petición de Oferta") {
            this.blockUI.start("Generando...");
            this.service.getPdfPeticionDeOfertaUsuario(legajoDto.ArchivoId)
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
        } else if (legajoDto.Tipo == "Chat Interno") {
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
        } else if (legajoDto.Tipo == "Cotización adjunto") {
            this.blockUI.start('Generando...');
            this.service.descargarAdjuntosProveedores(legajoDto.PeticionDeOfertaId, legajoDto.PeticionDeOfertaUsuarioId)
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
        } else if (legajoDto.Tipo == LegajoTipo.CotizacionAdjunto) {
            this.descargarAdjuntosProveedores(legajoDto)
        } else if (legajoDto.Tipo == LegajoTipo.Cotizacion && legajoDto.ArchivoId) {
            this.descargarHistorialCotizaciones(legajoDto.ArchivoId);
        }
        else if (legajoDto.Tipo == LegajoTipo.HistorialMovimientos) {
            this.descargarHistorialMovimientos();
        }
        else if (legajoDto.Tipo == LegajoTipo.RevisionTecnica && legajoDto.ArchivoId) {
            this.descargarRevisionTecnica(legajoDto.ArchivoId);
        }
        else {
            this.blockUI.start("Descargando...");
            this.service.DescargarArchivo(legajoDto.ArchivoId)
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

    descargarAdjuntosProveedores(legajoDto: LegajoDto) {
        this.blockUI.start('Generando...');
        this.service.descargarAdjuntosProveedores(legajoDto.PeticionDeOfertaId, legajoDto.PeticionDeOfertaUsuarioId)
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
        this.service.descargarLegajo(idPeticion, null, false,Number(this.adjudicacionId))
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
    private descargarHistorialCotizaciones(cotizacionId: number) {
        this.blockUI.start("Descargando...");
        this.service.descargarHistorialCotizaciones(cotizacionId).subscribe(
            (response) => {
                var byteArray = new Uint8Array(response.FileContents);
                var blob = new Blob([byteArray], {
                    type: "application/octet-stream",
                });
                this.downloadArchivoLocal(blob, response.FileDownloadName);
                this.blockUI.stop();
            },
            (error) => {
                this.mensajeComponent.setErrorMsg(error.message);
                this.blockUI.stop();
            }
        )
    }

    private descargarRevisionTecnica(peticionDeOfertaId: number) {
        this.blockUI.start("Descargando...");
        this.service.descargarRevisionTecnica(peticionDeOfertaId).subscribe(
            (response) => {
                var byteArray = new Uint8Array(response.FileContents);
                var blob = new Blob([byteArray], {
                    type: "application/octet-stream",
                });
                this.downloadArchivoLocal(blob, response.FileDownloadName);
                this.blockUI.stop();
            },
            (error) => {
                this.mensajeComponent.setErrorMsg(error.message);
                this.blockUI.stop();
            }
        )
    }
    descargarHistorialMovimientos() {
        this.blockUI.start("Descargando...");
        let idPeticion = this.legajo.ListaLegajos[0].PeticionDeOfertaId;
        this.service.descargarArchivoHistorialMovimientos(idPeticion)
            .subscribe(
                (result) => {
                    this.blockUI.stop();
                    var byteArray = new Uint8Array(result.FileContents);
                    var blob = new Blob([byteArray], {
                        type: "application/octet-stream",
                    });
                    this.downloadArchivoLocal(blob, result.FileDownloadName);
                    ;
                },
                (error) => {
                    this.mensajeComponent.setErrorMsg(error.message);
                    this.blockUI.stop();
                }
            )
    }

    getBackgroundColor(currentId: number): any {
        if (this.previousId !== currentId) {
            this.toggleColor = !this.toggleColor;
            this.previousId = currentId;
        }
        return {
            'background-color': this.toggleColor ? '#c9c9c9' : '#afafaf', 
            'color': 'back',
            //'font-weight': 'bold', 
        };
    }

}