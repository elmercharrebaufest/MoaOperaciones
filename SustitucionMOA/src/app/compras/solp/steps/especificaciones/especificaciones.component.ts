import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, FormControl, AbstractControl, ValidationErrors } from '@angular/forms';
import ImageResize from 'quill-image-resize-module';
import Quill from 'quill';
import { ConfirmationService } from 'primeng/api';

import { ListBaseComponent } from '../../../../common/base-components/list-base-component'
import { SessionDataService } from '../../../../common/services/SessionDataService';
import { SecurityService } from '../../../../common/services/SecurityService';
import { NavService } from '../../../../common/services/NavService';
import { FloatMsgService } from '../../../../common/services/FloatMsgService';
import { ModalService } from '../../../../common/services/ModalService';
import { ComprasService } from '../../../compras.service'
import { Solp } from '../../solp';
import { EspecificacionesViewModel } from './especificacionesViewModel';
import { ValidadorPasoSolpService } from '../../../validadorPasoSolpService';
import { EnumPasoSolp } from '../../../enum-paso-solp';

Quill.register('modules/imageResize', ImageResize);

declare var $: any;

@Component({
    selector: 'especificaciones',
    templateUrl: `especificaciones.component.html`,
    styleUrls: ['../../../compras.component.css'],
})
export class EspecificacionesComponent extends ListBaseComponent {

    @Input('model')
    protected model: Solp;

    @Input('locale')
    protected locale: any;

    //variables auxiliares de text rich
    posicionDeInicioInsert: number = 0;
    public viewModel: EspecificacionesViewModel;
    modulesEditor = {};

    formularioEspecificaciones: FormGroup;

    @Output() onEstCompleto = new EventEmitter<any>();

    mostrarRadioUsarTemplateCondicionesGenerales: boolean = false;

    constructor(protected service: ComprasService, protected navService: NavService, protected sessionDataService: SessionDataService,
        protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        protected route: ActivatedRoute, protected router: Router, private formBuilder: FormBuilder,
        private validadorPasoSolpService: ValidadorPasoSolpService, private confirmationService: ConfirmationService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);

        this.modulesEditor = {
            imageResize: true
        }
    }

    mostrarError(nombreCampo: string): boolean {
        if (this.formularioEspecificaciones && this.formularioEspecificaciones.controls) {
            return (this.formularioEspecificaciones.controls[nombreCampo].invalid || (this.formularioEspecificaciones.controls[nombreCampo].errors && this.formularioEspecificaciones.controls[nombreCampo].errors.required))
                && (this.formularioEspecificaciones.controls[nombreCampo].dirty || this.formularioEspecificaciones.controls[nombreCampo].touched)
        }

        return false;
    }

    setTabs() {
        //this.setMenuSeccionTab("Especificaciones", "Especificaciones");
    }

    ngOnInit() {
        this.setTabs();
        this.viewModel = this.model.especificacionesViewModel;

        //declaro las validaciones para los campos
        this.formularioEspecificaciones = this.formBuilder.group({
            observacion: new FormControl(this.viewModel.valorPorDefecto, [this.validatorObservaciones(this.viewModel.valorPorDefecto)]),
        });

        this.validadorPasoSolpService.formulario = this.formularioEspecificaciones
        if (this.model.cargoPasoTres) {
            this.validadorPasoSolpService.aplicarValidaciones();
        }

        this.model.cargoPasoTres = true;
    }

    validatorObservaciones(valorPorFecto: string): any {
        return (control: AbstractControl): ValidationErrors | null => {
            if (control.value == undefined || control.value == "" || control.value == valorPorFecto) {
                return { 'requerid': true };
            }
            return null;
        };
    }

    //elimno el archivo, llamar al servicio de eliminacion
    eliminarAdjuntoNuevo(archivo): void {
        var indice = this.viewModel.archivosEspecificacionesNuevos.indexOf(archivo)
        this.viewModel.archivosEspecificacionesNuevos.splice(indice, 1)
    }

    eliminarAdjuntoGuardado(archivo): void {
        var indice = this.viewModel.archivosEspecificaciones.indexOf(archivo)
        this.viewModel.archivosEspecificaciones.splice(indice, 1)
    }

    descargarArchivo(archivo): void {
        if (archivo.id != undefined) {
            this.service.DescargarArchivo(archivo.id)
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
                    },
                    (error) => {
                        this.spinnerSmallComponent.hideIt();
                        this.mensajeComponent.setErrorMsg(error.message);
                    }
                )
        }
        else {
            this.downloadArchivoLocal(archivo, archivo.name);
        }
    }

    uploadHandler(filesUpload: any): void {
        this.viewModel.archivosEspecificacionesNuevos = filesUpload["files"];
        var archivoWeb = this.viewModel.archivosEspecificacionesNuevos.reduce((sum, file) => sum + file.size, 0);
        if (archivoWeb > 10000000) {
            if (this.viewModel.archivosEspecificaciones.length > 0) {
                this.eliminarAdjuntoNuevo(this.viewModel.archivosEspecificaciones[this.viewModel.archivosEspecificaciones.length - 1])
            }
            this.floatMsgService.setErrorMsg("El archivo adjuntado no debe superar los 10Mb");
        }
    }

    selectionChange(event): void {
        if (event.range && this.viewModel.observaciones) {
            this.posicionDeInicioInsert = this.ObtenerPosicionInsert(event.range.index, this.viewModel.observaciones);
        }
    }

    fileChange(file): void {
        if (this.posicionDeInicioInsert != undefined && this.viewModel.observaciones.length > this.posicionDeInicioInsert) {
            var textoInicial = this.viewModel.observaciones.substring(0, this.posicionDeInicioInsert + 1);
            var textoFinal = this.viewModel.observaciones.substring(this.posicionDeInicioInsert + 1, this.viewModel.observaciones.length);
            this.viewModel.observaciones = textoInicial + '<img src=' + file + '>' + textoFinal;
            this.posicionDeInicioInsert = undefined;
        }
        else {
            this.viewModel.observaciones = this.viewModel.observaciones + '<img src=' + file + '>';
        }
    }

    ObtenerPosicionInsert(posicion: number, texto: string): number {
        let contar = false;
        for (var i = 0; i < texto.length; i++) {
            var letra = texto[i];
            if (letra == "<") {
                contar = false;
                continue
            }
            else if (letra == ">") {
                contar = true;
                continue;
            }

            if (contar) {
                posicion--;
            }

            if (posicion == 0)
                return i;
        }
    }

    ngOnDestroy() {
        super.ngOnDestroy();
        this.onEstCompleto.emit({ codigo: EnumPasoSolp.PliegoEspecificacion, esPasoInvalido: this.validadorPasoSolpService.esPasoInvalido() });
    }

    private downloadArchivoLocal(blob: Blob, nombreArchivo: string): void {
        if (window.navigator.msSaveOrOpenBlob) {
            // IE11
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

    eliminarArchivo(esAdjuntoNuevo: boolean, archivo: any) {
        this.confirmationService.confirm({
            message: '¿Está seguro de que desea eliminar el archivo?',
            accept: () => {
                esAdjuntoNuevo ? this.eliminarAdjuntoNuevo(archivo) : this.eliminarAdjuntoGuardado(archivo)
            },
            reject: () => {

            }
        });
    }

}