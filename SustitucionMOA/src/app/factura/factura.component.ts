import { Component, OnInit, ViewChild, OnDestroy, ElementRef } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { FacturaService } from './factura.service';
import { SpinnerComponent } from './../common/view-child/spinner/spinner.component';
import { MensajeComponent } from './../common/view-child/mensaje/mensaje.component';
import { ListBaseComponent } from './../common/base-components/list-base-component'
import { NavService } from './../common/services/NavService';
import { FloatMsgService } from './../common/services/FloatMsgService';
import { SecurityService } from './../common/services/SecurityService';
import { Seccion } from './../common/models/seccion';
import { BaseComponent } from './../common/base-components/base-component';
import { SessionDataService } from './../common/services/SessionDataService';
import { ModalService } from './../common/services/ModalService';
import { SpinnerSmallComponent } from './../common/view-child/spinner-small/spinner-small.component';
import { DropdownComponent, DropdownOption } from './../common/view-child/dropdown/dropdown.component';
import { ReCaptchaComponent } from 'angular2-recaptcha';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { ValidationResult } from '../common/models/validationResult';
import { FileUpload } from 'primeng/fileupload';
import { Certificacion } from './factura.model';


declare var $: any;


@Component({
    selector: 'app-factura',
    templateUrl: `factura.component.html`,
    providers: [FacturaService]
})
export class FacturaComponent extends ListBaseComponent {
    @BlockUI() blockUI: NgBlockUI;

    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerSmallComponent)
    public spinnerSmallComponent: SpinnerSmallComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    @ViewChild('recaptchaComponent')
    protected captcha: ReCaptchaComponent;
    resultados: ValidationResult[];

    @ViewChild('fileUpload') fileUpload: FileUpload;
    constructor(protected service: FacturaService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService, protected route: ActivatedRoute, protected router: Router) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
        this.spinnerSmallComponent = new SpinnerSmallComponent();
    }

    checkPermisos() { this.securityService.tienePermisoRedirect("CARGAR FACT PROV"); }

    modalServiceSusbcription: any;
    captchaOk: any = null;
    archivos = new Array<File>()
    certificaciones: Certificacion[] = [];
    agrupadasPorArchivo: { nombreArchivo: string, items: Certificacion[] }[] = [];

    certificacionesAgregadas: Certificacion[] = [];

    certificacionesRegistradasExistentes = [];
    verPendientes: boolean = false;

    setTabs() {
        this.setMenuSeccionTab("factura", "Factura");
    }

    ngOnInit() {
        this.setTabs();
        this.checkPermisos();
        let secciones = [];
        secciones.push(new Seccion('/factura', 'factura', 'Factura'));
        this.navService.setSeccionList(secciones);
    }

    obtenerCertificaciones() {
        this.floatMsgService.setMsgsEmpty();
        this.spinnerSmallComponent.showIt();

        if (this.archivos == null) {
            this.spinnerSmallComponent.hideIt();
            this.floatMsgService.setErrorMsg("Ingrese un Archivo");
            return false;
        }

        this.unsubscribe();
        try {
            this.resultados = [];
            this.certificaciones = [];
            this.certificacionesAgregadas = [];
            this.certificacionesRegistradasExistentes = [];
            this.blockUI.start('Analizando documentos...');

            this.subscription = this.service.subirPDF(this.archivos).subscribe({
                next: (result: any) => {
                    this.spinnerSmallComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        try {
                            this.resultados = result.data as ValidationResult[];
                            this.resultados.forEach(resultado => {
                                const fileName = resultado.FileName;
                                const nroOC = resultado.Value;
                                if (resultado.Certificaciones != null && resultado.Certificaciones.length > 0) {
                                    resultado.Certificaciones.forEach(certificacion => {
                                        this.certificaciones.push({
                                            NombreDeArchivo: fileName || "",
                                            NRO_OC: nroOC || "",
                                            NRO_Certificacion: certificacion.NroCertificacion,
                                            Importe: certificacion.Saldo,
                                            Moneda: certificacion.Moneda,
                                            MontoFormateado: certificacion.MontoFormateado,
                                            Archivo: certificacion.Archivo,
                                            Seleccionada: false
                                        });
                                    });

                                    // Agrupar justo después de cargar
                                    const agrupadas = new Map<string, any[]>();

                                    for (const cert of this.certificaciones) {
                                        if (!agrupadas.has(cert.NombreDeArchivo)) {
                                            agrupadas.set(cert.NombreDeArchivo, []);
                                        }
                                        agrupadas.get(cert.NombreDeArchivo)!.push(cert);
                                    }

                                    this.agrupadasPorArchivo = Array.from(agrupadas.entries()).map(([nombreArchivo, items]) => ({
                                        nombreArchivo,
                                        items
                                    }));

                                }

                            });
                        } catch (error) {
                            console.log(error);
                            this.certificaciones = [];
                        }
                    }
                    this.blockUI.stop();
                },
                error: (error) => {
                    let errormsj = "Ha ocurrido un error, por favor intentelo nuevamente";
                    this.spinnerSmallComponent.hideIt();
                    this.floatMsgService.setErrorMsg(errormsj);
                    this.blockUI.stop();
                }
            });
        } catch (e) {
            this.spinnerSmallComponent.hideIt();
            this.floatMsgService.setErrorMsg(e);
            this.blockUI.stop();
            return false;
        }
        return false;
    }

    obtenerNombreDeArchivo(ruta: string) {
        // Replace all backslashes with forward slashes and then split
        const normalizedPath = ruta.replace(/\\/g, '/');
        const partes = normalizedPath.split('/');
        return partes[partes.length - 1];
    }

    // formatPrice(importe: number) {
    //     return importe.toLocaleString('es-AR', { style: 'currency', currency: 'ARS' });
    // }

    onCheckCertificacion(certificacionSeleccionada: Certificacion) {
        // Verificar si la certificación ya fue registrada con uno o más archivos
        if (certificacionSeleccionada.Seleccionada) {
            if (certificacionSeleccionada.Archivo != null && certificacionSeleccionada.Archivo.length > 0) {
                this.floatMsgService.setInfoMsg(
                    `Advertencia: La certificación ${certificacionSeleccionada.NRO_Certificacion} ya está vinculada a otra factura.`
                );
            }
        }

    }

    alMenosUnaSeleccionadaPorGrupo(): boolean {
        return !this.agrupadasPorArchivo.every(grupo =>
            grupo.items.some(cert => cert.Seleccionada)
        );
    }

    actualizarCertificacionesAgregadas(): void {
        this.certificacionesAgregadas = [];

        for (const grupo of this.agrupadasPorArchivo) {
            const seleccionadas = grupo.items.filter(cert => cert.Seleccionada);
            this.certificacionesAgregadas.push(...seleccionadas);
        }
    }

    registrarCertificaciones() {
        this.floatMsgService.setMsgsEmpty();
        this.spinnerSmallComponent.showIt();
        this.unsubscribe();
        try {
            this.blockUI.start('Enviando factura...');
            this.actualizarCertificacionesAgregadas();
            // Filtrar archivos por nombre de archivo que esten en el array de certificaciones agregadas
            this.archivos = this.archivos.filter(archivo => this.certificacionesAgregadas.map(certificacion => certificacion.NombreDeArchivo).includes(archivo.name));
            this.subscription = this.service.registrarCertificaciones(this.certificacionesAgregadas, this.archivos).subscribe(
                (result: any) => {
                    this.spinnerSmallComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.floatMsgService.setMsgsEmpty();
                        this.floatMsgService.setSuccessMsg("Factura enviada correctamente para su análisis");
                        this.certificacionesAgregadas = [];
                        this.certificaciones = [];
                    }
                    this.blockUI.stop();
                    this.vaciarCampos();
                    return false;
                },
                error => {
                    let errormsj = "Ha ocurrido un error, por favor inténtelo nuevamente";
                    this.spinnerSmallComponent.hideIt();
                    this.floatMsgService.setErrorMsg(errormsj);
                    this.blockUI.stop();
                }
            );
        } catch (e) {
            this.spinnerSmallComponent.hideIt();
            this.floatMsgService.setErrorMsg(e);
            this.blockUI.stop();
            return false; //<-- Prevent Refresh
        }
    }

    descargarDocumentoAdjunto(archivoId: number) {
        this.blockUI.start("Descargando...");
        this.service.descargarDocumentoAdjunto(archivoId.toString())
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

                        if (window.navigator.msSaveOrOpenBlob) {
                            // IE11
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
                    }
                },
                (error) => {
                    this.mensajeComponent.setErrorMsg(error.message);
                    this.blockUI.stop();
                }
            )
    }

    mensajeIrAOC(mensaje: ValidationResult) {
        return !mensaje.IsValid && mensaje.ValidataionType == "OrdenCompraValidationCommand" && mensaje.Value != "" && mensaje.Value.length > 0;
    }

    irACertificaciones(nroOC: string) {
        this.navService.navegarSeccionParam('/compras/dashboardCertificacionDeServiciosProveedores', nroOC);
        return false;
    }

    handleCorrectCaptcha(event: any) {
        this.captchaOk = event;
    }

    vaciarCampos() {
        this.fileUpload.clear();
        this.archivos = new Array<File>();
        this.certificaciones = [];
        this.certificacionesAgregadas = [];
        this.certificacionesRegistradasExistentes = [];
        this.resultados = [];
    }

    public ngOnDestroy() {
        this.floatMsgService.setMsgsEmpty();
        this.unsubscribe();
        if (this.modalServiceSusbcription != undefined) {
            this.modalServiceSusbcription.unsubscribe();
        }
    }

    uploadHandler(filesUpload: any): boolean {
        this.removeFile(null);
        for (let file of this.archivos) {
            if (!this.isValidFileType(file)) {
                this.floatMsgService.setErrorMsg(`El archivo ${file.name} no es válido. Solo se permiten archivos PDF, JPG, JPEG y PNG.`);
                this.eliminarAdjuntoNuevo(file);
                return true;
            }
        }

        let archivoWeb = filesUpload["files"].reduce((sum, file) => sum + file.size, 0);
        this.archivos = filesUpload["files"];
        if (archivoWeb > 10000000) {
            this.floatMsgService.setErrorMsg("El archivo adjuntado no debe superar los 10Mb");
            if (this.archivos.length > 0) {
                this.eliminarAdjuntoNuevo(this.archivos[this.archivos.length - 1])
            }
            return true;
        }
    }

    isValidFileType(file: File): boolean {
        const validTypes = ['.pdf', '.jpg', '.jpeg', '.png'];
        const fileExtension = file.name.split('.').pop().toLowerCase();
        return validTypes.includes(`.${fileExtension}`);
    }

    eliminarAdjuntoNuevo(archivo): void {
        let indice = this.archivos.indexOf(archivo)
        this.archivos.splice(indice, 1)
    }

    removeFile(event: any) {
        this.certificaciones = [];
        this.certificacionesAgregadas = [];
        this.certificacionesRegistradasExistentes = [];
        this.resultados = [];
    }

    verCertificacionesPendientes() {
        this.verPendientes = !this.verPendientes;
        this.desmarcarTodasLasCertificaciones();
    }

    desmarcarTodasLasCertificaciones(): void {
        this.agrupadasPorArchivo.forEach(grupo => {
            grupo.items.forEach(certificacion => {
                certificacion.Seleccionada = false;
            });
        });
    }

    todasSeleccionadas(grupo: { items: Certificacion[] }): boolean {
        const itemsAConsiderar = this.verPendientes
            ? grupo.items.filter(cert => !cert.Archivo || cert.Archivo.length === 0)
            : grupo.items;

        return itemsAConsiderar.length > 0 && itemsAConsiderar.every(cert => cert.Seleccionada);
    }

    toggleSeleccionGrupo(grupo: { items: Certificacion[] }, event: Event): void {
        const checked = (event.target as HTMLInputElement).checked;

        grupo.items.forEach(cert => {
            if (!this.verPendientes || !cert.Archivo || cert.Archivo.length === 0) {
                cert.Seleccionada = checked;
            }
        });
    }

    contarSeleccionadas(grupo: { items: Certificacion[] }): number {
        return grupo.items.filter(cert => cert.Seleccionada).length;
    }

    totalImporteSeleccionadas(grupo: { items: Certificacion[] }): string {
        const total = grupo.items
            .filter(cert => cert.Seleccionada)
            .reduce((sum, cert) => sum + (cert.Importe || 0), 0);

        return grupo.items[0].Moneda + " " + total.toLocaleString('es-AR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    }

}