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

    constructor(protected service: SolpService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService, protected route: ActivatedRoute, protected router: Router) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }

    setTabs() {
        this.setMenuSeccionTab("Especificaciones", "Especificaciones");
    }

    ngOnInit() {
        this.setTabs();

        //borrar cuando se implemente servicio de carga de datos
        this.viewModel = this.model.especificacionesViewModel;
        this.viewModel.archivosGuardadosEspecificaciones = [
            {
                id: 0,
                nombreArchivo: "Archivo 1",
                rutaDeAcceso: "C:/Imagen/archivo"
            },
            {
                id: 1,
                nombreArchivo: "Archivo 2",
                rutaDeAcceso: "C:/Imagen/archivo"
            },
            {
                id: 2,
                nombreArchivo: "Archivo 3",
                rutaDeAcceso: "C:/Imagen/archivo"
            },
            {
                id: 3,
                nombreArchivo: "Archivo 4",
                rutaDeAcceso: "C:/Imagen/archivo"
            },
            {
                id: 5,
                nombreArchivo: "Archivo 5",
                rutaDeAcceso: "C:/Imagen/archivo"
            }
        ]

    }

    //elimno el archivo, llamar al servicio de eliminacion
    eliminarAdjunto(archivo): void {
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
        this.viewModel.archivosAdjuntos = filesUploaad;
    }

    clearFile(evento:any): void{
        var indice = this.viewModel.archivosAdjuntos["files"].indexOf(evento.file);
        this.viewModel.archivosAdjuntos["files"].splice(indice, 1)
    }

    selectionChange(event): void {
        if (event.range && this.viewModel.observaciones) {
            this.posicionDeInicioInsert = this.ObtenerPosicionInsert(event.range.index, this.viewModel.observaciones);
        }
    }

    fileChange(file): void {
        if (this.posicionDeInicioInsert != undefined) {
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

}
