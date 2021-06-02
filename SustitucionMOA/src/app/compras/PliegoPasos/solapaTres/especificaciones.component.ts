import { Component, ElementRef, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { ListBaseComponent } from '../../../common/base-components/list-base-component'
import { SessionDataService } from '../../../common/services/SessionDataService';
import { SecurityService } from '../../../common/services/SecurityService';
import { NavService } from '../../../common/services/NavService';
import { FloatMsgService } from '../../../common/services/FloatMsgService';
import { ModalService } from '../../../common/services/ModalService';
import { ComprasService } from '../../compras.service'
import { Solp } from '../../Solp';
import { EspecificacionesViewModel } from './especificacionesViewModel';
import  ImageResize  from 'quill-image-resize-module';
import Quill from 'quill';
import {FormBuilder, FormGroup, FormControl, AbstractControl, ValidationErrors } from '@angular/forms';
import { ValidadorPasoSolpService } from '../../validadorPasoSolpService';
import { EnumPasoSolp } from '../../enum-paso-solp';

 Quill.register('modules/imageResize', ImageResize);

declare var $: any;

@Component({
    selector: 'especificaciones',
    templateUrl: `especificaciones.component.html`,
    styleUrls: ['../../compras.component.css'],
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

    formularioEspecificaciones : FormGroup;

    @Output() onEstCompleto = new EventEmitter<any>();

    constructor(protected service: ComprasService, protected navService: NavService, protected sessionDataService: SessionDataService, 
        protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService,
         protected route: ActivatedRoute, protected router: Router,private formBuilder: FormBuilder,
         private validadorPasoSolpService : ValidadorPasoSolpService) {
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
        this.setMenuSeccionTab("Especificaciones", "Especificaciones");
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
            if (control.value == undefined || control.value =="" || control.value == valorPorFecto) {
                return { 'requerid': true };
            }
            return null;
        };
    }
    

    //elimno el archivo, llamar al servicio de eliminacion
    eliminarAdjuntoNuevo(archivo): void {
        var indice = this.viewModel.archivosAdjuntosNuevos.indexOf(archivo)
        this.viewModel.archivosAdjuntosNuevos.splice(indice, 1)
    }

    eliminarAdjuntoGuardado(archivo): void {
        var indice = this.viewModel.archivosGuardadosEspecificaciones.indexOf(archivo)
        this.viewModel.archivosGuardadosEspecificaciones.splice(indice, 1)
    }

    descargarArchivo(archivo): void {
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
                            return false;
                        }
                    }
                },
                (error) => {
                    this.spinnerSmallComponent.hideIt();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            )
    }

    uploadHandler(filesUploaad: any): void {
        this.viewModel.archivosAdjuntosNuevos = filesUploaad["files"];
    }

    selectionChange(event): void {
        if (event.range && this.viewModel.observaciones) {
            this.posicionDeInicioInsert = this.ObtenerPosicionInsert(event.range.index, this.viewModel.observaciones);
        }
    }

    fileChange(file): void {
        if (this.posicionDeInicioInsert != undefined && this.viewModel.observaciones.length > this.posicionDeInicioInsert) {
                var textoInicial = this.viewModel.observaciones.substring(0,  this.posicionDeInicioInsert + 1);
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

    ngOnDestroy()
    {
        super.ngOnDestroy();
        this.onEstCompleto.emit({codigo :EnumPasoSolp.PliegoEspecificacion, esPasoInvalido : this.validadorPasoSolpService.esPasoInvalido()});
    }

}
