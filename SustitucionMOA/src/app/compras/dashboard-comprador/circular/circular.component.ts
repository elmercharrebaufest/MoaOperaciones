import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges, ViewChild } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbAlert } from '@ng-bootstrap/ng-bootstrap';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { ConfirmationService } from 'primeng/api';
import { FloatMsgService } from '../../../common/services/FloatMsgService';
import { ModalService } from '../../../common/services/ModalService';
import { NavService } from '../../../common/services/NavService';
import { SecurityService } from '../../../common/services/SecurityService';
import { SessionDataService } from '../../../common/services/SessionDataService';
import { CircularDto } from '../../../modelos/circular-model';
import { PeticionDeOfertaDto } from '../../../modelos/peticion-de-oferta-model';
import { ComprasService } from '../../compras.service';

@Component({
    selector: 'app-circular',
    templateUrl: './circular.component.html',
    styleUrls: ['./circular.component.css']
})
export class CircularComponent implements OnInit, OnChanges {


    @Input()
    displayCircular: boolean;
    @Input('locale') es: any;

    @Input()
    public peticion: PeticionDeOfertaDto;
    fechaDeEntrega: Date
    plazoDeOferta: Date
    public circular: CircularDto;
    @Output() cerrarCircularEmitter = new EventEmitter();
    archivos = new Array<File>()
    val1: string = "No";
    val2: string;
    Observacion: any;
    visualizarFechas: boolean;
    plazoDias: string;
    selectedProv: number[] = []
    nroCircular: any;
    displayOkCircular: boolean;
    subscription: any;
    @BlockUI() blockUI: NgBlockUI;
    error: string = "";
    visualizarAlert = false;
    hoy: Date = new Date();
    constructor(protected service: ComprasService, protected navService: NavService, protected sessionDataService: SessionDataService,
        protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        protected route: ActivatedRoute, protected router: Router, private confirmationService: ConfirmationService, private formBuilder: FormBuilder) {
    }
    ngOnChanges(changes: SimpleChanges): void {
        if(this.peticion != null){
        this.selectedProv = this.peticion.Usuarios.map(x => x.UsuarioId);
        }
    }
    
    ngOnInit() {
        this.es = {
            firstDayOfWeek: 1,
            dayNames: ["domingo", "lunes", "martes", "miércoles", "jueves", "viernes", "sábado"],
            dayNamesShort: ["dom", "lun", "mar", "mié", "jue", "vie", "sáb"],
            dayNamesMin: ["D", "L", "M", "X", "J", "V", "S"],
            monthNames: ["enero", "febrero", "marzo", "abril", "mayo", "junio", "julio", "agosto", "septiembre", "octubre", "noviembre", "diciembre"],
            monthNamesShort: ["ene", "feb", "mar", "abr", "may", "jun", "jul", "ago", "sep", "oct", "nov", "dic"],
            today: 'Hoy',
            clear: 'Borrar'
        }
    }

    autocompletarFechaDeEntrega(){
        if (this.peticion != null && this.peticion.FechaEntregaFormateado != "") {
            const date = new Date(this.peticion.FechaEntregaFormateado);
            this.fechaDeEntrega = date;
        }
    }

    onCerrarCircular() {
        this.visualizarAlert = false;
        this.iniciarModalCircular();
        this.cerrarCircularEmitter.next();
    }

    grabarCircular() {
        this.armarCircular();
        this.validarCircular();
        if(!this.visualizarAlert){
        this.blockUI.start("Grabando...");
        try {           
            this.subscription = this.service.GrabarCircular(this.circular).subscribe(
                    (result: any) => {
                        if (result.logout == true) {
                            this.sessionDataService.logout();
                        } else if (result.error != undefined && result.error != "") {
                            this.error = result.error;
                            this.visualizarAlert = true;
                           // this.floatMsgService.setErrorMsg(result.error);
                        } else if (result.info != undefined) {
                            this.error = result.info;
                           this.visualizarAlert = true;
                          //  this.floatMsgService.setInfoMsg(result.info);
                        } else {
                            this.nroCircular = result.data.IdEntidad;
                            this.displayOkCircular = true;
                        }
                        this.blockUI.stop();
                    },
                    error => {
                        //this.floatMsgService.setErrorMsg(error.message);
                        this.error = error.message;
                        this.visualizarAlert = true;
                        this.blockUI.stop();
    
                    });
            } catch (e) {
                //this.floatMsgService.setErrorMsg(e);
                this.error = e;
                this.visualizarAlert = true;
                this.blockUI.stop();
                return false; //<-- Prevent Refresh
            }
            return false; //<-- Prevent Refresh
        }
    }

   

    uploadHandler(filesUpload: any): boolean {
        this.visualizarAlert = false; 
        var archivoWeb = filesUpload["files"].reduce((sum, file) => sum + file.size, 0);      
        this.archivos = filesUpload["files"];
        if(archivoWeb > 10000000){
            this.error = "El archivo adjuntado no debe superar los 10Mb";
            this.eliminarAdjuntoNuevo(this.archivos[this.archivos.length - 1])
            return  this.visualizarAlert = true;             
        }
    }

    eliminarArchivo(archivo: any) {
        this.confirmationService.confirm({
            message: '¿Está seguro que desea eliminar el archivo?',
            accept: () => {
                this.eliminarAdjuntoNuevo(archivo);
                this.visualizarAlert = false;
            },
            reject: () => {
               
            }
        });
    }

    eliminarAdjuntoNuevo(archivo): void {
        var indice = this.archivos.indexOf(archivo)
        this.archivos.splice(indice, 1)
    }

    eliminarTodosLosArchivos(){
        this.archivos.splice(0, this.archivos.length)
    }

    descargarArchivo(archivo) {
        this.downloadArchivoLocal(archivo, archivo.name);
    }

    estaSeleccionado(seleccion) {
        this.visualizarFechas = seleccion == "No" ? false : true;
        if(!this.visualizarFechas){
            this.borrarFechas();
        }else{
            this.autocompletarFechaDeEntrega();
        }
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

    private armarCircular() {
        let c: CircularDto = {
            Adjuntos: this.archivos,
            FechaEntrega: this.fechaDeEntrega,
            PlazoDeOferta: this.plazoDeOferta,
            Observacion: this.Observacion,
            RequiereCambioDeFecha: this.visualizarFechas,
            UsuarioIds: this.selectedProv,
            PeticionDeOferta_Id: this.peticion.Id
        };
        this.circular = c;
    }

    calcularFechaEntrega() {
        let fechaNueva = new Date(this.fechaDeEntrega);

        if(this.plazoDias != null && this.plazoDias != undefined && this.plazoDias != ""){
            fechaNueva.setDate(fechaNueva.getDate() + parseInt(this.plazoDias));
            this.fechaDeEntrega = fechaNueva;
        }

        if(this.plazoDias == ""){
            this.autocompletarFechaDeEntrega();
        }
    } 

    borrarFechas(){
        this.plazoDeOferta = null;
        this.fechaDeEntrega = null;
        this.plazoDias = null;
    }

    iniciarModalCircular() {
        this.Observacion = "";
        this.visualizarFechas = false;        
        this.selectedProv = [];
        this.val1 = 'No';
        this.val2 = '';
        this.borrarFechas();
        this.autocompletarFechaDeEntrega();
        this.eliminarTodosLosArchivos();
    }

    salir(){
        this.onCerrarCircular(); 
        this.displayOkCircular = false;
    }

    validarCircular(){
        if(this.Observacion == "" || this.Observacion == undefined){
            this.error = "El campo Observacion es obligatorio";
           return this.visualizarAlert = true;
        }
        if((this.fechaDeEntrega == null || this.fechaDeEntrega == undefined) && this.visualizarFechas){
            this.error = "Debe completar la Fecha de entrega";
            return this.visualizarAlert = true;
        }
        if((this.plazoDeOferta == null || this.plazoDeOferta == undefined) && this.visualizarFechas){
            this.error = "Debe completar el Plazo de oferta";
            return this.visualizarAlert = true;
        }
        if(this.selectedProv == null || this.selectedProv.length == 0){
            this.error = "Debe seleccionar al menos un proveedor";
            return  this.visualizarAlert = true;
        }
        var archivoWeb = this.archivos.reduce((sum, file) => sum + file.size, 0);      
        if(archivoWeb > 10000000){
           this.error = "El archivo adjuntado no debe superar los 10Mb";
            return  this.visualizarAlert = true;             
        }
        return this.visualizarAlert = false;
    }
}
