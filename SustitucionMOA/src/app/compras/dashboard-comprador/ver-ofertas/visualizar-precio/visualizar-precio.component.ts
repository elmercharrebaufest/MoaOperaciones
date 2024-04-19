import { Component, EventEmitter, Input, OnChanges, OnDestroy, OnInit, Output, SimpleChanges } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfirmationService } from 'primeng/api';
import { AdjudicacionDto } from '../../../../modelos/adjudicacion';
import { ComprasService } from '../../../compras.service';
import { NavService } from '../../../../common/services/NavService';
import { SecurityService } from '../../../../common/services/SecurityService';
import { FloatMsgService } from '../../../../common/services/FloatMsgService';
import { ModalService } from '../../../../common/services/ModalService';
import { SessionDataService } from '../../../../common/services/SessionDataService';
import { PeticionVisualizacionPrecioDto } from '../../../../modelos/peticion-visualizar-precio-dto';
import { BlockUI, NgBlockUI } from 'ng-block-ui';

@Component({
    selector: 'app-visualizar-precio',
    templateUrl: './visualizar-precio.component.html',
    styleUrls: ['./visualizar-precio.component.css']
})
export class VisualizarPrecioComponent implements OnInit {

    @Input()
    displayVisualizarPrecio: boolean;   
    @Output() cerrarHistorialEmitter = new EventEmitter();

    peticionVisualizacionPrecio: PeticionVisualizacionPrecioDto
    visualizarAlert: boolean;
    error: string;
    nroPeticion: any;
    displayOkPeticion: boolean;
    subscription: any;

    @Input()
    peticionOferta_Id: number; 
    @BlockUI() blockUI: NgBlockUI;
    constructor(protected service: ComprasService, protected navService: NavService, protected sessionDataService: SessionDataService,
        protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        protected route: ActivatedRoute, protected router: Router, private confirmationService: ConfirmationService, private formBuilder: FormBuilder) {
    }   
    

    ngOnInit() {       
        
        this.peticionVisualizacionPrecio = {
            Observacion: "",
            PeticionOfertaId: this.peticionOferta_Id,
        };
    }
    
    onCerrarPrecios() {            
        this.cerrarHistorialEmitter.next();      
    }
        
    uploadHandler(filesUpload: any): boolean {
        this.visualizarAlert = false;
        var archivoWeb = filesUpload["files"].reduce((sum, file) => sum + file.size, 0);
        this.peticionVisualizacionPrecio.Adjuntos = filesUpload["files"];
        if (archivoWeb > 10000000) {
            this.error = "El archivo adjuntado no debe superar los 10Mb";
            if (this.peticionVisualizacionPrecio.Adjuntos.length > 0) {
                this.eliminarAdjuntoNuevo(this.peticionVisualizacionPrecio.Adjuntos[this.peticionVisualizacionPrecio.Adjuntos.length - 1])
            }
            return this.visualizarAlert = true;
        }
    }

    eliminarAdjuntoNuevo(archivo): void {
        var indice = this.peticionVisualizacionPrecio.Adjuntos.indexOf(archivo)
        this.peticionVisualizacionPrecio.Adjuntos.splice(indice, 1)
    }

    descargarArchivo(archivo) {
        this.downloadArchivoLocal(archivo, archivo.name);
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

    eliminarArchivo(archivo: any) {
        this.confirmationService.confirm({
            message: '¿Está seguro de que desea eliminar el archivo?',
            accept: () => {
                this.eliminarAdjuntoNuevo(archivo);
                this.visualizarAlert = false;
            },
            reject: () => {

            }
        });
    }

    grabarPeticionDeOfertaVisualizacionPrecio() {
    
       this.validarPeticionDeOfertaVisualizacionPrecio();
       this.peticionVisualizacionPrecio.PeticionOfertaId = this.peticionOferta_Id
        if (!this.visualizarAlert) {
            this.blockUI.start("Grabando...");
            try {
                this.subscription = this.service.GrabarPeticionDeOfertaVisualizacionPrecio(this.peticionVisualizacionPrecio).subscribe(
                    (result: any) => {
                        if (result.logout == true) {
                            this.sessionDataService.logout();
                        } else if (result.error != undefined && result.error != "") {
                            this.error = result.error;
                            this.visualizarAlert = true;
                        } else if (result.info != undefined) {
                            this.error = result.info;
                            this.visualizarAlert = true;
                        } else {
                            this.nroPeticion = result.data.IdEntidad;
                            this.displayOkPeticion = true;
                        }
                       this.blockUI.stop();
                    },
                    error => {                       
                        this.error = error.message;
                        this.visualizarAlert = true;
                        this.blockUI.stop();
                    });
            } catch (e) {               
                this.error = e;
                this.visualizarAlert = true;
                this.blockUI.stop();
                return false; 
            }
            return false; 
        }
    }

    salir() {
        this.onCerrarPrecios();
        this.displayOkPeticion = false;
        this.router.navigateByUrl('/', { skipLocationChange: true }).then(() => {
            this.router.navigate(['compras/ver-ofertas/' + this.peticionOferta_Id]);
          });
    }

    validarPeticionDeOfertaVisualizacionPrecio() {
        this.visualizarAlert = false;
        if (!this.peticionVisualizacionPrecio.Observacion || this.peticionVisualizacionPrecio.Observacion.trim() === '') {            
            this.error = "La observación es obligatoria";
            this.visualizarAlert = true;
        } 
        if(this.peticionVisualizacionPrecio.Adjuntos == undefined){
            this.error = "Debe adjuntar un archivo";
            this.visualizarAlert = true;
        }
         if (this.peticionVisualizacionPrecio.Adjuntos != undefined && this.peticionVisualizacionPrecio.Adjuntos.length > 1) {
            this.error = "Solo se permite adjuntar un archivo.";
            this.visualizarAlert = true;
        } 
        return this.visualizarAlert;
    }
    
 
}
