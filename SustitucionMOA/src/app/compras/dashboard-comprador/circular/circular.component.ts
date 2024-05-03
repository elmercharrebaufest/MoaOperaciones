import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges, ViewChild } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
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

    @Input() displayCircular: boolean;
    @Input('locale') es: any;
    @Input() public peticion: PeticionDeOfertaDto;
    @Input() solicitante: boolean;

    fechaDeEntrega: Date
    plazoDeOfertaFecha: Date
    plazoDeOfertaHora: Date

    public circular: CircularDto;
    @Output() cerrarCircularEmitter = new EventEmitter();
    @Output() onCloseModalEmitter = new EventEmitter();
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
        if (this.solicitante) {
            if (this.peticion != null) {
                this.selectedProv = this.peticion.Usuarios.map(x => x.UsuarioId);
            }
        } else {
            if (this.peticion != null) {
                if (this.peticion.TipoPosicionCodigo == "SERVICIO") {
                    if (this.peticion.RevisionFinalizada) {
                        this.selectedProv = this.peticion.Usuarios
                            .filter(x => x.PropuestaTecnicaAprobada)
                            .map(x => x.UsuarioId);
                    }
                    // Establecer la propiedad Deshabilitado para los usuarios que no cumplen la condición
                    this.peticion.Usuarios.forEach(x => x.Deshabilitado = !this.selectedProv.includes(x.UsuarioId));
                } else {
                    this.selectedProv = this.peticion.Usuarios
                        .filter(x => x.Cotizacion.CotizacionEstado_Id == 1
                            && (this.peticion.RespetaMateriales == true
                                || (this.peticion.RespetaMateriales == false
                                    && (this.peticion.RevisionFinalizada && x.PropuestaTecnicaAprobada))))
                        .map(x => x.UsuarioId);

                    // Establecer la propiedad Deshabilitado para los usuarios que no cumplen la condición
                    this.peticion.Usuarios.forEach(x => x.Deshabilitado = !this.selectedProv.includes(x.UsuarioId));
                }
            }
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

        this.plazoDeOfertaHora = new Date(1, 1, 1, 12, 0, 0, 0);
    }

    autocompletarFechaDeEntrega() {
        if (this.peticion != null && this.peticion.FechaEntregaFormateado != "") {
            // Separar la fecha en sus componentes (año, mes, día)
            const parts = this.peticion.FechaEntregaFormateado.split('-');
            if (parts.length === 3) {
                const year = parseInt(parts[0], 10);
                const month = parseInt(parts[1], 10) - 1; // Los meses en JavaScript van de 0 a 11
                const day = parseInt(parts[2], 10);

                // Crear una nueva fecha con los componentes
                const date = new Date(year, month, day);

                // La variable 'date' ahora contiene la fecha deseada
                this.fechaDeEntrega = date;
            } else {
                console.error('El formato de la fecha no es válido');
            }
        }
    }

    onCerrarCircular() {
        this.visualizarAlert = false;
        this.iniciarModalCircular();
        this.onCloseModalEmitter.next();
    }

    grabarCircular() {
        this.armarCircular();
        this.validarCircular();
        if (!this.visualizarAlert) {
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
        if (archivoWeb > 10000000) {
            this.error = "El archivo adjuntado no debe superar los 10Mb";
            if (this.archivos.length > 0) {
                this.eliminarAdjuntoNuevo(this.archivos[this.archivos.length - 1])
            }
            return this.visualizarAlert = true;
        }
    }

    eliminarArchivoCircular(archivo: any) {
        this.confirmationService.confirm({
            key: "eliminarArchivoCircular",
            message: '¿Está seguro de que desea eliminar el archivo?',
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

    eliminarTodosLosArchivos() {
        this.archivos.splice(0, this.archivos.length)
    }

    descargarArchivo(archivo) {
        this.downloadArchivoLocal(archivo, archivo.name);
    }

    estaSeleccionado(seleccion) {
        this.visualizarFechas = seleccion == "No" ? false : true;
        if (!this.visualizarFechas) {
            this.borrarFechas();
        } else {
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
            PlazoDeOfertaFecha: this.plazoDeOfertaFecha,
            PlazoDeOfertaHora: this.plazoDeOfertaHora,
            Observacion: this.Observacion,
            RequiereCambioDeFecha: this.visualizarFechas,
            UsuarioIds: this.selectedProv,
            PeticionDeOferta_Id: this.peticion.Id
        };
        this.circular = c;
    }

    calcularFechaEntrega() {
        let fechaNueva = new Date(this.fechaDeEntrega);

        if (this.plazoDias != null && this.plazoDias != undefined && this.plazoDias != "") {
            fechaNueva.setDate(fechaNueva.getDate() + parseInt(this.plazoDias));
            this.fechaDeEntrega = fechaNueva;
        }

        if (this.plazoDias == "") {
            this.autocompletarFechaDeEntrega();
        }
    }

    borrarFechas() {
        this.plazoDeOfertaFecha = null;
        this.plazoDeOfertaHora = null;
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

    salirCircular() {
        this.visualizarAlert = false;
        this.iniciarModalCircular();
        this.cerrarCircularEmitter.next();
        this.displayOkCircular = false;
    }

    validarCircular() {
        if (this.Observacion == "" || this.Observacion == undefined) {
            this.error = "El campo Observaciones es obligatorio";
            return this.visualizarAlert = true;
        }
        if ((this.fechaDeEntrega == null || this.fechaDeEntrega == undefined) && this.visualizarFechas) {
            this.error = "Debe completar la Fecha de entrega";
            return this.visualizarAlert = true;
        }
        if ((this.plazoDeOfertaFecha == null || this.plazoDeOfertaFecha == undefined) && this.visualizarFechas) {
            this.error = "Debe completar el Plazo de oferta";
            return this.visualizarAlert = true;
        }
        if (this.selectedProv == null || this.selectedProv.length == 0) {
            this.error = "Debe seleccionar al menos un proveedor";
            return this.visualizarAlert = true;
        }
        var archivoWeb = this.archivos.reduce((sum, file) => sum + file.size, 0);
        if (archivoWeb > 10000000) {
            this.error = "El archivo adjuntado no debe superar los 10Mb";
            return this.visualizarAlert = true;
        }
        return this.visualizarAlert = false;
    }
}