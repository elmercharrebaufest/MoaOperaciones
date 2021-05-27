import { Component, ElementRef, Input, ViewChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { ListBaseComponent } from '../../../common/base-components/list-base-component'
import { SessionDataService } from '../../../common/services/SessionDataService';
import { SecurityService } from '../../../common/services/SecurityService';
import { NavService } from '../../../common/services/NavService';
import { FloatMsgService } from '../../../common/services/FloatMsgService';
import { ModalService } from '../../../common/services/ModalService';
import { SolpService } from '../../solp.service'
import { Solp } from '../../Solp';
import { EspecificacionesViewModel } from './especificacionesViewModel';
import  ImageResize  from 'quill-image-resize-module';
import Quill from 'quill';
import { IValidadorPasoSolp } from '../../IValidadorPasoSolp';
import {FormBuilder, FormGroup, FormControl,Validators } from '@angular/forms';

 Quill.register('modules/imageResize', ImageResize);

declare var $: any;

@Component({
    selector: 'especificaciones',
    templateUrl: `especificaciones.component.html`,
    styleUrls: ['../../compras.component.css'],
})
export class EspecificacionesComponent extends ListBaseComponent  implements IValidadorPasoSolp {

    @Input('model')
    protected model: Solp;

    @Input('locale')
    protected locale: any;

    //variables auxiliares de text rich
    posicionDeInicioInsert: number = 0;
    public viewModel: EspecificacionesViewModel;
    modulesEditor = {};

    formularioEspecificaciones : FormGroup;

    constructor(protected service: SolpService, protected navService: NavService, protected sessionDataService: SessionDataService, 
        protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService,
         protected route: ActivatedRoute, protected router: Router,private formBuilder: FormBuilder) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);

        this.modulesEditor = {
            imageResize: true
        }
    }

    aplicarValidaciones(): void {
        Object.keys(this.formularioEspecificaciones.controls).forEach(key => {
            let control = this.formularioEspecificaciones.get(key);
            control.markAsDirty();
            control.updateValueAndValidity();
          });
    }
    mostrarError(nombreCampo: string): boolean {
        if (this.formularioEspecificaciones && this.formularioEspecificaciones.controls) {
            return (this.formularioEspecificaciones.controls[nombreCampo].invalid || (this.formularioEspecificaciones.controls[nombreCampo].errors && this.formularioEspecificaciones.controls[nombreCampo].errors.required))
                && (this.formularioEspecificaciones.controls[nombreCampo].dirty || this.formularioEspecificaciones.controls[nombreCampo].touched)
        }

        return false;
    }

    esPasoInvalido(): boolean {
        let pasoValido = true;

        if (this.viewModel.observaciones || this.viewModel.observaciones.trim() == "" || this.viewModel.ObservacionesEsValorPorDefecto()) {
            pasoValido = false;
        }

        return pasoValido;
    }

    setTabs() {
        this.setMenuSeccionTab("Especificaciones", "Especificaciones");
    }

    ngOnInit() {
        this.setTabs();
        this.viewModel = this.model.especificacionesViewModel;

         //declaro las validaciones para los campos
         this.formularioEspecificaciones = this.formBuilder.group({
            observacion: new FormControl(this.viewModel.valorPorDefecto, [Validators.required]),
        });
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
        //cuando el servicio este disponible descomentar la logica
        // this.service.DescargarArchivo(archivoId)
        //     .subscribe(
        //         (result) => {
        //             if (result.logout == true) {
        //                 this.sessionDataService.logout();
        //             }
        //             else {
        //                 var byteArray = new Uint8Array(result.FileContents);
        //                 var blob = new Blob([byteArray], {
        //                     type: "application/octet-stream",
        //                 });

        //                 if (window.navigator.msSaveOrOpenBlob) {
        //                     // IE11
        //                     window.navigator.msSaveOrOpenBlob(
        //                         blob,
        //                         result.FileDownloadName
        //                     );
        //                 } else {
        //                     var url = window.URL.createObjectURL(blob);
        //                     var link = document.createElement("a");
        //                     document.body.appendChild(link);
        //                     link.href = url;
        //                     link.download = result.FileDownloadName;
        //                     link.click();
        //                     setTimeout(function () {
        //                         window.URL.revokeObjectURL(url);
        //                     }, 0);
        //                     return false;
        //                 }
        //             }
        //         },
        //         (error) => {
        //             this.spinnerSmallComponent.hideIt();
        //             this.mensajeComponent.setErrorMsg(error.message);
        //         }
        //     )
       
        var blob = new Blob(['Hello, world!'], {type: 'text/plain'});

        if (window.navigator.msSaveOrOpenBlob) {
            // IE11
            window.navigator.msSaveOrOpenBlob(
                blob,
                "Test"
            );
        } else {
            var url = window.URL.createObjectURL(blob);
            var link = document.createElement("a");
            document.body.appendChild(link);
            link.href = url;
            link.download = "Test";
            link.click();
            setTimeout(function () {
                window.URL.revokeObjectURL(url);
            }, 0);
        }
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

}
