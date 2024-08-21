import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ListBaseComponent } from '../../../common/base-components/list-base-component';
import { ActivatedRoute, Router } from '@angular/router';
import { FloatMsgService } from '../../../common/services/FloatMsgService';
import { ModalService } from '../../../common/services/ModalService';
import { NavService } from '../../../common/services/NavService';
import { SecurityService } from '../../../common/services/SecurityService';
import { SessionDataService } from '../../../common/services/SessionDataService';
import { ComprasService } from '../../compras.service';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { LegajoDto, LegajoTipo } from '../../../modelos/compras/legajoDto';

@Component({
    selector: 'app-legajo',
    templateUrl: './legajo.component.html',
    styleUrls: ['./legajo.component.css']
})
export class LegajoComponent extends ListBaseComponent implements OnInit {

    @Input() displayLegajo: boolean;
    @Input() usuarioProveedor: boolean;
    @Input() legajo: LegajoDto[];
    @Input() esAuditor: boolean;
    @Input() idLegajo: number;


    @Output() cerrarLegajoEmitter = new EventEmitter();
    @Output() descargarLegajoEmitter = new EventEmitter();
    @Output() descargarArchivoEmitter = new EventEmitter<{ archivoId: number }>();
    @Output() adjuntarArchivoLegajoEmitter = new EventEmitter<{ files: any }>();

    @BlockUI() blockUI: NgBlockUI;

    error: string;
    visualizarAlert = false;


    constructor(protected service: ComprasService, protected navService: NavService,
        protected sessionDataService: SessionDataService, protected securityService: SecurityService,
        protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        protected route: ActivatedRoute, protected router: Router) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }

    ngOnInit() {
    }

    onCerrarLegajo() {
        this.cerrarLegajoEmitter.next();
    }

    onHideLeajoDialog() {
        this.cerrarLegajoEmitter.next();
    }

    // descargarArchivo(archivoId: number) {
    //     this.descargarArchivoEmitter.next({ archivoId: archivoId });
    // }

    descargarLegajo() {
        this.descargarLegajoEmitter.next();
    }
    onBasicUploadAuto(event, fileUpload) {
        var archivoWeb = event.files.reduce((sum, file) => sum + file.size, 0);
        if (archivoWeb > 10000000) {
            this.error = "El archivo adjuntado no debe superar los 10Mb";
            fileUpload.clear();
            return this.visualizarAlert = true;
        } else {
            this.adjuntarArchivoLegajoEmitter.next(event.files);
            fileUpload.clear();
            this.visualizarAlert = false;
        }
    }

    descargarArchivo(archivoId: number | undefined, tipoLegajo: string) {
        if (tipoLegajo == LegajoTipo.Solp || tipoLegajo == LegajoTipo.Pliego) {
            this.descargarSolp_Pliego(this.legajo[0].SolpId);
        }
        else if (tipoLegajo == LegajoTipo.PeticionDeOferta && archivoId) {
            this.descargarPeticionDeOferta(archivoId);
        }
        else if (tipoLegajo == LegajoTipo.ChatInterno) {
            this.descargarChatInterno(this.legajo[0].SolpId);
        }
        else if ((tipoLegajo == LegajoTipo.SolpArchivos || tipoLegajo == LegajoTipo.Legajo || tipoLegajo == LegajoTipo.Circular) && archivoId) {
            this.descargarSolpArchivos_Legajo_Circular(archivoId);
        }
        else if (tipoLegajo == LegajoTipo.Cotizacion && archivoId) {
            this.descargarHistorialCotizaciones(archivoId);
        } else if (tipoLegajo == LegajoTipo.HistorialMovimientos) {
            this.descargarHistorialMovimientos();
        }
    }

    private descargarSolp_Pliego(solpId: number) {
        this.blockUI.start("Generando...");
        this.service.getPdf(solpId)
            .subscribe(
                (result) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    }
                    else {
                        var byteArray = new Uint8Array(result.FileContents);
                        var blob = new Blob([byteArray], {
                            type: "application/octet-stream",
                        });

                        this.downloadArchivoLocal(blob, result.FileDownloadName);
                    }
                    this.blockUI.stop();
                },
                (error) => {
                    this.blockUI.stop();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            )
    }

    private descargarPeticionDeOferta(archivoId: number) {
        this.blockUI.start("Generando...");
        this.service.getPdfPeticionDeOfertaUsuario(archivoId)
            .subscribe(
                (result) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    }
                    else {
                        var byteArray = new Uint8Array(result.FileContents);
                        var blob = new Blob([byteArray], {
                            type: "application/octet-stream",
                        });

                        this.downloadArchivoLocal(blob, result.FileDownloadName);
                    }
                    this.blockUI.stop();
                },
                (error) => {
                    this.blockUI.stop();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            )
    }

    private descargarChatInterno(solpId: number) {
        this.blockUI.start("Generando...");
        this.service.obtenerYExportarChat(solpId.toString())
            .subscribe(
                (result) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    }
                    else {
                        var byteArray = new Uint8Array(result.FileContents);
                        var blob = new Blob([byteArray], {
                            type: "text/plain",
                        });

                        this.downloadArchivoLocal(blob, result.FileDownloadName);
                    }
                    this.blockUI.stop();
                },
                (error) => {
                    this.blockUI.stop();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            )
    }

    private descargarSolpArchivos_Legajo_Circular(archivoId: number) {
        this.blockUI.start("Descargando...");
        this.service.DescargarArchivo(archivoId)
            .subscribe(
                (result) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    }
                    else {
                        var byteArray = new Uint8Array(result.FileContents);
                        var blob = new Blob([byteArray], {
                            type: "application/octet-stream",
                        });
                        this.downloadArchivoLocal(blob, result.FileDownloadName);
                        this.blockUI.stop();
                    }
                },
                (error) => {
                    this.mensajeComponent.setErrorMsg(error.message);
                    this.blockUI.stop();
                }
            )
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

    private downloadArchivoLocal(blob: Blob, nombreArchivo: string): void {
        if ((window.navigator as any).msSaveOrOpenBlob) {
            // IE11
            (window.navigator as any).msSaveOrOpenBlob(blob, nombreArchivo);
        }
        else {
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
    descargarHistorialMovimientos() {
        this.blockUI.start("Descargando...");
        this.service.descargarArchivoHistorialMovimientos(this.idLegajo)
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
}
