import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, FormControl, Validators, AbstractControl } from '@angular/forms';
import { ConfirmationService, MessageService } from 'primeng/api';

import { ListBaseComponent } from '../../../../common/base-components/list-base-component'
import { SessionDataService } from '../../../../common/services/SessionDataService';
import { SecurityService } from '../../../../common/services/SecurityService';
import { NavService } from '../../../../common/services/NavService';
import { FloatMsgService } from '../../../../common/services/FloatMsgService';
import { ModalService } from '../../../../common/services/ModalService';
import { ComprasService } from '../../../compras.service'
import { Solp } from '../../solp';
import { ValidadorPasoSolpService } from '../../../validadorPasoSolpService';
import { EnumPasoSolp } from '../../../enum-paso-solp';
import { AltaNuevoProveedor } from '../../../solp-compra';
import { OrdenDeCompraSap } from '../../../../modelos/ordenDeCompraSap';

declare var $: any;

@Component({
    selector: 'cotizacion',
    templateUrl: `cotizacion.component.html`,
    styleUrls: ['../../../compras.component.css'],
    providers: [ComprasService, MessageService]
})
export class CotizacionComponent extends ListBaseComponent {

    @Input('model')
    protected model: Solp;

    @Input('locale')
    protected locale: any;

    //validaciones
    formularioCotizacion: FormGroup;

    camposObligatorios: any[] = [
        { campo: 'dias', esObligatorio: true}
    ];

    proveedorSeleccionado: any;

    proveedores: any[] = new Array();

    estaFinalizada: boolean;
    @Output() ordenDeCompraSap: OrdenDeCompraSap;

    @Output() onEstCompleto = new EventEmitter<any>();
    mostrar: boolean;

    constructor(protected service: ComprasService, protected navService: NavService, protected sessionDataService: SessionDataService,
        protected securityService: SecurityService, protected floatMsgService: FloatMsgService,
        protected modalService: ModalService, protected route: ActivatedRoute, protected router: Router
        , private formBuilder: FormBuilder, private confirmationService: ConfirmationService,
        private validadorPasoSolpService : ValidadorPasoSolpService, private messageService: MessageService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }

    mostrarValidacion(campoAValidar){
        let camposVacios = this.camposObligatorios.find(x => x.campo == campoAValidar && x.esObligatorio);
        return (camposVacios != null && this.mostrarError(campoAValidar) !== null);
    }

    mostrarError(nombreCampo: string): boolean {
        if (this.formularioCotizacion && this.formularioCotizacion.controls) {
            return (this.formularioCotizacion.controls[nombreCampo].invalid || (this.formularioCotizacion.controls[nombreCampo].errors && this.formularioCotizacion.controls[nombreCampo].errors.required))
                && (this.formularioCotizacion.controls[nombreCampo].dirty || this.formularioCotizacion.controls[nombreCampo].touched)
        }

        return false;
    }

    validatorDias(control: AbstractControl): { [key: string]: boolean } | null {
        let diasNoSeleccionados = 0;
        for (let index = 0; index < control.value.length; index++) {
            const dia =  control.value[index];
            if (!dia.selected) {
                diasNoSeleccionados = diasNoSeleccionados + 1;
            }
        }

        if (control.value.length == diasNoSeleccionados) {
            return { 'requerid': true };
        }

        return null;
    }

    ejecucion: number;
    comienzoJornadaLaboral: Date;
    terminoJornadaLaboral: Date;


    setTabs() {
        this.setMenuSeccionTab("Cotizacion", "Cotizacion");
    }

    ngOnInit() {
        this.setTabs();

      
        //declaro las validaciones para los campos
        this.formularioCotizacion = this.formBuilder.group({
            //ejecucion: new FormControl('', [Validators.required]),
            comienzoJornadaLaboral: new FormControl('', Validators.required),
            terminoJornadaLaboral: new FormControl('', Validators.required),
            dias: new FormControl(this.model.jornadaLaboralDias, [Validators.required, this.validatorDias]),
            trabajoHecho: new FormControl('', Validators.required),
            proveedorSeleccionado: new FormControl('', Validators.required),    
            adicional: new FormControl('', Validators.required),
            ordenDeCompra: new FormControl({value: '', disabled: this.estaFinalizada}, Validators.required)
        });

        this.validadorPasoSolpService.formulario = this.formularioCotizacion;
        if (this.model.cargoPasoCuatro) {
            this.validadorPasoSolpService.aplicarValidaciones();
        }

        this.model.cargoPasoCuatro = true;
        
        if(this.model.proveedorAsignado_Id){
            this.proveedorSeleccionado = {
                Id: this.model.proveedorAsignado_Id,
                RazonSocial: this.model.proveedorAsignado
            }
        }

        if(this.model.ordenDeCompra){
            this.obtenerOrdenDeCompra();
        } else {
            this.model.ordenDeCompra = "";
        }

        if(this.model.nroSolp ){
            this.estaFinalizada = true
        } else {
            this.estaFinalizada = false
        }

        this.validacionTrabajoHecho();
    }

    ngOnDestroy()
    {
        super.ngOnDestroy();
        this.onEstCompleto.emit({codigo :EnumPasoSolp.PliegoCotizacion, esPasoInvalido : this.validadorPasoSolpService.esPasoInvalido()});
    }

    uploadHandler(filesUpload: any): void {
        this.model.archivosCotizacionesNuevos = filesUpload["files"];
        var archivoWeb = this.model.archivosCotizacionesNuevos.reduce((sum, file) => sum + file.size, 0);      
        if(archivoWeb > 10000000){     
            if (this.model.archivosCotizacionesNuevos.length > 0){       
            this.eliminarAdjuntoNuevo(this.model.archivosCotizacionesNuevos[this.model.archivosCotizacionesNuevos.length - 1])
            }
            this.floatMsgService.setErrorMsg("El archivo adjuntado no debe superar los 10Mb");          
        }
        this.validacionTrabajoHecho();
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

    eliminarAdjuntoNuevo(archivo): void {
        var indice = this.model.archivosCotizacionesNuevos.indexOf(archivo)
        this.model.archivosCotizacionesNuevos.splice(indice, 1)
        this.validacionTrabajoHecho();
    }

    eliminarAdjuntoGuardado(archivo): void {
        var indice = this.model.archivosCotizaciones.indexOf(archivo)
        this.model.archivosCotizaciones.splice(indice, 1)
        this.validacionTrabajoHecho();
    }

    eliminarArchivo(esAdjuntoNuevo: boolean, archivo: any) {
        this.confirmationService.confirm({
            message: '�Est� seguro que desea eliminar el archivo?',
            accept: () => {
                esAdjuntoNuevo ? this.eliminarAdjuntoNuevo(archivo) : this.eliminarAdjuntoGuardado(archivo)
            },
            reject: () => {

            }
        });
        this.validacionTrabajoHecho();

    }

    searchProveedor(event) {
        try {
            this.subscription = this.service.listarProveedores(event.query).subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.proveedores = result.data;
                    }
                },
                error => {
                    this.floatMsgService.setErrorMsg(error.message);
                });
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    }
    
    selectProveedor(event) {
        try {
            this.model.proveedorAsignado_Id = event.Id;
            this.model.proveedorAsignado = event.RazonSocial;
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
        }
    }

    validacionTrabajoHecho(){
        this.model.validarTrabajoHecho = true;

        if(this.model.trabajoHecho == true || this.model.adicional == true){

            if(this.model.observacionesCotizacion == ""  || this.model.observacionesCotizacion == undefined || this.model.observacionesCotizacion == null){
                this.model.mensajeCotizacion = "Debe agregar una observación en el paso #4";
                this.messageService.add({ severity: 'error', summary: 'No se pudo finalizar', detail: `${this.model.mensajeCotizacion}` });
                this.model.validarTrabajoHecho = false;
                
            } 
                
            if((this.model.archivosCotizaciones == null || this.model.archivosCotizaciones.length == 0) && (this.model.archivosCotizacionesNuevos == null || this.model.archivosCotizacionesNuevos.length == 0)){
                this.model.mensajeCotizacion = "Debe adjuntar un archivo en el paso #4";
                this.messageService.add({ severity: 'error', summary: 'No se pudo finalizar', detail: `${this.model.mensajeCotizacion}` });
                this.model.validarTrabajoHecho = false;
                
            }

            if(this.model.trabajoHecho == true){
                if (!this.proveedorSeleccionado || this.proveedorSeleccionado == "" || typeof this.proveedorSeleccionado === "undefined")
                {
                    this.model.mensajeCotizacion = "Debe agregar un proveedor en el paso #4";
                    this.messageService.add({ severity: 'error', summary: 'No se pudo finalizar', detail: `${this.model.mensajeCotizacion}` });
                    this.model.validarTrabajoHecho = false;
                    
                }
            }

            if(this.model.adicional == true){
                if (!this.model.ordenDeCompra || this.model.ordenDeCompra == "" || typeof this.model.ordenDeCompra === "undefined")
                {
                    this.model.mensajeCotizacion = "Debe agregar un numero de OC en el paso #4";
                    this.messageService.add({ severity: 'error', summary: 'No se pudo finalizar', detail: `${this.model.mensajeCotizacion}` });
                    this.model.validarTrabajoHecho = false;
                    
                }
            }
            
        }  
        return this.model.validarTrabajoHecho;
    }

    limpiarCheck(){
        if(this.model.trabajoHecho == undefined || this.model.trabajoHecho == false){
            if(!this.estaFinalizada){
                this.model.proveedorAsignado = "";
                this.model.proveedorAsignado_Id = null;
                this.proveedorSeleccionado = null;
            }
        }
    }

    limpiarCheckAdicional(){
        if(!this.estaFinalizada && this.model.ordenDeCompra != ""){
            this.model.ordenDeCompra = "";
            this.ordenDeCompraSap.Cabecera.RazonSocialProveedor = "";
            this.ordenDeCompraSap.Cabecera.CodigoProveedor = "";
            this.ordenDeCompraSap.Cabecera.OrdenDeCompra = "";
            this.model.proveedorAsignado = "";
        }
    }


    obtenerOrdenDeCompra() {
        try {
            if(this.model.ordenDeCompra.length >= 10){
                this.subscription = this.service.obtenerOrdenDeCompra(this.model.ordenDeCompra).subscribe(
                    (result: any) => {
                        if (result.logout == true) {
                            this.sessionDataService.logout();
                        } else if (result.error != undefined && result.error != "") {
                            this.floatMsgService.setErrorMsg(result.error);
                        } else if (result.info != undefined) {
                            this.floatMsgService.setInfoMsg(result.info);
                        } else {
                            this.ordenDeCompraSap = result.data;
                            this.model.proveedorAsignado_Id = this.ordenDeCompraSap.Cabecera.Usuario_Id;
                            this.model.ordenDeCompra = this.ordenDeCompraSap.Cabecera.OrdenDeCompra;
                            this.model.proveedorAsignado = this.ordenDeCompraSap.Cabecera.RazonSocialProveedor;
                            if(this.ordenDeCompraSap.Error){
                                this.floatMsgService.setErrorMsg(this.ordenDeCompraSap.Error.Mensaje);      
                                this.limpiarCheckAdicional();                      
                            }
                        }
                    },
                    error => {
                        this.floatMsgService.setErrorMsg(error.message);
                    });
            }
            this.limpiarCheck();           
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    }


    validarCondiciones(campoCheck){
        if(this.estaFinalizada == true){
            return true;
        }
        if(this.model.adicional == true && campoCheck == 'trabajoHecho'){
            return true; 
        } 

        if(this.model.trabajoHecho == true && campoCheck == 'adicional'){
            return true;
        }
        return false
    }
}
