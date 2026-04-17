import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, FormControl, Validators, AbstractControl } from '@angular/forms';
import { ConfirmationService, MessageService, SelectItem } from 'primeng/api';
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
import { OrdenDeCompraSap } from '../../../../modelos/ordenDeCompraSap';
import { CondicionesEspecialesOriginales } from './condiciones-especiales-originales';
import { Permiso } from '../../../../common/enums/Permisos';

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

    @Input('condEspOriginales')
    protected condEspOriginales: CondicionesEspecialesOriginales;

    @Output() ordenDeCompraSap: OrdenDeCompraSap;
    @Output() onEstCompleto = new EventEmitter<any>();
    formularioCotizacion: FormGroup;

    camposObligatorios: any[] = [
        { campo: 'dias', esObligatorio: true }
    ];

    esUsuarioRRHH: boolean = this.isAuthorized(Permiso.ComprasRRHH);

    proveedorSeleccionado: any;
    proveedores: any[] = new Array();
    estaFinalizada: boolean;
    mostrar: boolean;
    liberadoresJefes: SelectItem[] = [];
    liberadoresGerentes: SelectItem[] = [];
    liberadoresDirectores: SelectItem[] = [];
    selectJefes: number[] = [];
    selectGerentes: number[] = [];
    selectDirectores: number[] = [];
    hoy: Date = new Date();
    condicionEspecial: boolean;
    condicionEspecialOriginal: boolean;
    ajustePolinomicaDisabled: boolean;
    infoBoxText: string;
    mostrarInfoBox: boolean = false;

    constructor(protected service: ComprasService, protected navService: NavService, protected sessionDataService: SessionDataService,
        protected securityService: SecurityService, protected floatMsgService: FloatMsgService,
        protected modalService: ModalService, protected route: ActivatedRoute, protected router: Router
        , private formBuilder: FormBuilder, private confirmationService: ConfirmationService,
        private validadorPasoSolpService: ValidadorPasoSolpService, private messageService: MessageService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }

    mostrarValidacion(campoAValidar) {
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
            const dia = control.value[index];
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
        //this.setMenuSeccionTab("Cotizacion", "Cotizacion");
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
            conPresupuesto: new FormControl('', Validators.required),
            seraUsadoEnPliegoMultiple: new FormControl('', Validators.required),
            certificacionAutomatica: new FormControl('', Validators.required),
            proveedorSeleccionado: new FormControl('', Validators.required),
            adicional: new FormControl('', Validators.required),
            ordenDeCompra: new FormControl({ value: '', disabled: this.model.deshabilitarAdicional }, Validators.required),
            urgencia: new FormControl('', Validators.required),
            jefes: new FormControl('', Validators.required),
            gerentes: new FormControl('', Validators.required),
            directores: new FormControl('', Validators.required),
            condEspProveedorAsignado: new FormControl('', Validators.required),
            servicioPermanente: [{ value: this.model.thServicioPermanente }, []],
            ajustePolinomica: [{ value: this.model.thAjustePolinomica }, []],
            proveedorDirecto: [{ value: this.model.thProveedorDirecto }, []],
            acuerdoMarco: [{ value: this.model.thAcuerdoMarco }, []],
        });

        this.validadorPasoSolpService.formulario = this.formularioCotizacion;
        if (this.model.cargoPasoCuatro) {
            this.validadorPasoSolpService.aplicarValidaciones();
        }

        this.model.cargoPasoCuatro = true;

        if (this.model.proveedorAsignado_Id) {
            this.proveedorSeleccionado = {
                Id: this.model.proveedorAsignado_Id,
                RazonSocial: this.model.proveedorAsignado,
                CodigoProveedorSap: this.model.codigoProveedorSap
            }
        }

        if (this.model.ordenDeCompra) {
            this.obtenerOrdenDeCompra();
            this.habilitarOC();
        } else {
            this.model.ordenDeCompra = "";
        }

        if (this.model.nroSolp) {
            this.estaFinalizada = true
        } else {
            this.estaFinalizada = false
        }

        this.listarLiberadorSap();

        if (this.model.thAjustePolinomica != true && this.model.thProveedorDirecto != true && this.model.thServicioPermanente != true && this.model.thAcuerdoMarco != true) {
            this.onRadioButtonChange("Servicio permanente");
        }

        this.adjustFormControlsBasedOnConditions();
    }

    ngOnDestroy() {
        super.ngOnDestroy();
        this.onEstCompleto.emit({ codigo: EnumPasoSolp.PliegoCotizacion, esPasoInvalido: this.validadorPasoSolpService.esPasoInvalido() });
    }

    uploadHandler(filesUpload: any): void {
        this.model.archivosCotizacionesNuevos = filesUpload["files"];
        this.validarChecks();
    }

    uploadHandlerCondEsp(filesUpload: any): void {
        this.model.archivosCotizacionesNuevosCondEsp = filesUpload["files"];
        this.validarChecks();
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
        this.eliminarAdjuntoNuevoGenerico(archivo, 'archivosCotizacionesNuevos');
    }

    eliminarAdjuntoNuevoCondEsp(archivo): void {
        this.eliminarAdjuntoNuevoGenerico(archivo, 'archivosCotizacionesNuevosCondEsp');
    }

    eliminarAdjuntoNuevoGenerico(archivo, claveModelo: string): void {
        const indice = this.model[claveModelo].indexOf(archivo);
        if (indice > -1) {
            this.model[claveModelo].splice(indice, 1);
        }
        this.validarChecks();
    }

    eliminarAdjuntoGuardado(archivo): void {
        this.eliminarAdjuntoGuardadoGenerico(archivo, 'archivosCotizaciones');
    }

    eliminarAdjuntoGuardadoCondEsp(archivo): void {
        this.eliminarAdjuntoGuardadoGenerico(archivo, 'archivosCotizacionesCondEsp');
    }

    eliminarAdjuntoGuardadoGenerico(archivo, claveModelo: string): void {
        const indice = this.model[claveModelo].indexOf(archivo);
        if (indice > -1) {
            this.model[claveModelo].splice(indice, 1);
        }
        this.validarChecks();
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
        this.validarChecks();
    }

    eliminarArchivoCondEsp(esAdjuntoNuevo: boolean, archivo: any) {
        this.confirmationService.confirm({
            message: '¿Está seguro de que desea eliminar el archivo?',
            accept: () => {
                esAdjuntoNuevo ? this.eliminarAdjuntoNuevoCondEsp(archivo) : this.eliminarAdjuntoGuardadoCondEsp(archivo)
            },
            reject: () => {

            }
        });
        this.validarChecks();
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
            this.model.codigoProveedorSap = event.CodigoProveedor;
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
        }
    }

    validarChecks() {
        this.model.validacionCheck = true;

        if (this.model.trabajoHecho == true || this.model.conPresupuesto || this.model.adicional == true || this.model.urgencia == true || this.model.condEspProveedorAsignado == true) {

            if ((this.model.archivosCotizacionesCondEsp == null || this.model.archivosCotizacionesCondEsp.length == 0) && (this.model.archivosCotizacionesNuevosCondEsp == null || this.model.archivosCotizacionesNuevosCondEsp.length == 0)) {
                this.model.mensajeCotizacion = "Debe adjuntar un archivo en el paso #4";
                this.messageService.add({ severity: 'error', summary: 'No se pudo finalizar', detail: `${this.model.mensajeCotizacion}` });
                this.model.validacionCheck = false;
            }

            if (this.model.observacionesCotizacionCondEsp == "" || this.model.observacionesCotizacionCondEsp == undefined || this.model.observacionesCotizacionCondEsp == null) {
                this.model.mensajeCotizacion = "Debe agregar una observación en el paso #4";
                this.messageService.add({ severity: 'error', summary: 'No se pudo finalizar', detail: `${this.model.mensajeCotizacion}` });
                this.model.validacionCheck = false;
            }

            if ((this.model.trabajoHecho == true || this.model.conPresupuesto) && this.model.adicional != true || this.model.condEspProveedorAsignado == true) {
                if (!this.proveedorSeleccionado || this.proveedorSeleccionado == "" || typeof this.proveedorSeleccionado === "undefined") {
                    this.model.mensajeCotizacion = "Debe agregar un proveedor en el paso #4";
                    this.messageService.add({ severity: 'error', summary: 'No se pudo finalizar', detail: `${this.model.mensajeCotizacion}` });
                    this.model.validacionCheck = false;
                }
            }

            if (this.model.adicional == true) {
                if (!this.model.ordenDeCompra || this.model.ordenDeCompra == "" || typeof this.model.ordenDeCompra === "undefined") {
                    this.model.mensajeCotizacion = "Debe agregar un número de OC en el paso #4";
                    this.messageService.add({ severity: 'error', summary: 'No se pudo finalizar', detail: `${this.model.mensajeCotizacion}` });
                    this.model.validacionCheck = false;
                }
            }

            if (this.model.adicional == true) {
                if (this.model.ordenDeCompra == null || this.model.ordenDeCompra.length < 10) {
                    this.model.mensajeCotizacion = "Debe ingresar los 10 números de OC en el paso #4";
                    this.messageService.add({ severity: 'error', summary: 'No se pudo finalizar', detail: `${this.model.mensajeCotizacion}` });
                    this.model.validacionCheck = false;
                }
            }

            if (this.model.urgencia == true && this.model.trabajoHecho != true && !this.model.conPresupuesto && this.selectJefes.length == 0) {
                this.model.mensajeCotizacion = "Debe elegir al menos un jefe en el paso #4 para enviarle la notificación de urgencia";
                this.messageService.add({ severity: 'error', summary: 'No se pudo finalizar', detail: `${this.model.mensajeCotizacion}` });
                this.model.validacionCheck = false;
            }

            if (this.model.trabajoHecho == true) {
                if (this.model.thAjustePolinomica != true && this.model.thProveedorDirecto != true && this.model.thServicioPermanente != true && this.model.thAcuerdoMarco != true) {
                    this.model.mensajeCotizacion = "Debe elegir una categoría de trabajo ya hacho en el paso #4";
                    this.messageService.add({ severity: 'error', summary: 'No se pudo finalizar', detail: `${this.model.mensajeCotizacion}` });
                    this.model.validacionCheck = false;
                }
            }
        }
        return this.model.validacionCheck;
    }

    limpiarCheck() {
        if ((this.model.trabajoHecho == undefined || this.model.trabajoHecho == false) && this.model.editarCondicionesEspeciales && (this.model.condEspProveedorAsignado == false || this.model.condEspProveedorAsignado == undefined)) {
            this.model.proveedorAsignado = "";
            this.model.proveedorAsignado_Id = 0;
            this.proveedorSeleccionado = null;
            this.model.thAjustePolinomica = false;
            this.model.thProveedorDirecto = false;
            this.model.thAcuerdoMarco = false;
            this.model.thServicioPermanente = true;
        }
        this.model.certificacionAutomatica = this.model.trabajoHecho || false;
    }

    limpiarCheckProveedorAsignado() {
        if ((this.model.condEspProveedorAsignado == undefined || this.model.condEspProveedorAsignado == false) &&
            this.model.editarCondicionesEspeciales &&
            this.model.trabajoHecho == false &&
            this.model.conPresupuesto
        ) {
            this.model.proveedorAsignado = "";
            this.model.proveedorAsignado_Id = null;
            this.proveedorSeleccionado = null;
        }
    }

    limpiarCheckAdicional() {
        if (this.model.ordenDeCompra != "" && this.model.editarCondicionesEspeciales) {
            this.model.ordenDeCompra = "";
            this.ordenDeCompraSap.Cabecera.RazonSocialProveedor = "";
            this.ordenDeCompraSap.Cabecera.CodigoProveedor = "";
            this.ordenDeCompraSap.Cabecera.OrdenDeCompra = "";
            this.model.proveedorAsignado = "";
        }
    }

    obtenerOrdenDeCompra() {
        try {
            if (this.model.ordenDeCompra.length >= 10) {
                this.subscription = this.service.obtenerOrdenDeCompra(this.model.ordenDeCompra).subscribe(
                    (result: any) => {
                        if (result.logout == true) {
                            this.sessionDataService.logout();
                        } else if (result.error != undefined && result.error != "") {
                            this.floatMsgService.setErrorMsg(result.error);
                        } else if (result.info != undefined) {
                            this.floatMsgService.setInfoMsg(result.info);
                        } else {

                            if (this.estaFinalizada == true && this.model.proveedorIdAdicional && this.model.proveedorIdAdicional != result.data.Cabecera.Usuario_Id) {
                                this.floatMsgService.setErrorMsg("La OC ingresada debe ser para el proveedor " + this.model.proveedorRazonSocialAdicional);
                                this.model.ordenDeCompra = "";
                            } else {
                                this.ordenDeCompraSap = result.data;

                                if (this.ordenDeCompraSap.Error != null) {
                                    this.floatMsgService.setErrorMsg(this.ordenDeCompraSap.Error.Mensaje);
                                    this.limpiarCheckAdicional();
                                } else {
                                    this.model.proveedorAsignado_Id = this.ordenDeCompraSap.Cabecera.Usuario_Id;
                                    this.model.ordenDeCompra = this.ordenDeCompraSap.Cabecera.OrdenDeCompra;
                                    this.model.proveedorAsignado = this.ordenDeCompraSap.Cabecera.RazonSocialProveedor;
                                    this.model.monedaOC = this.ordenDeCompraSap.Cabecera.Moneda;
                                    this.model.usuarioComprasId = this.ordenDeCompraSap.Cabecera.UsuarioCompras_Id;

                                    this.model.selectUsuarioCompras = this.model.usuarioComprasId > 0
                                        ? this.model.usuarioComprasList.find(x => x.Id === this.model.usuarioComprasId)
                                        : this.model.usuarioComprasList[0];
                                }
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

    listarLiberadorSap() {
        if (this.model.urgencia == true) {
            try {
                this.subscription = this.service.listarLiberadorSap().subscribe(
                    (result: any) => {
                        if (result.logout == true) {
                            this.sessionDataService.logout();
                        } else if (result.error != undefined && result.error != "") {
                            this.floatMsgService.setErrorMsg(result.error);
                        } else if (result.info != undefined) {
                            this.floatMsgService.setInfoMsg(result.info);
                        } else {
                            const listaResult = result.data;
                            //tildo en los select los que son obligatorios:
                            this.selectJefes = listaResult.filter(g => g.Cargo === 'Jefe' && g.Obligatorio).map(j => j.Id);
                            this.selectGerentes = listaResult.filter(g => g.Cargo === 'Gerente' && g.Obligatorio).map(g => g.Id);
                            this.selectDirectores = listaResult.filter(g => g.Cargo === 'Director' && g.Obligatorio).map(d => d.Id);
                            //los agrego a la solp:
                            const todosSelect = [...this.selectJefes, ...this.selectGerentes, ...this.selectDirectores];
                            const nuevos = todosSelect.filter(Id => !this.model.liberadoresSap.some(lib => lib.LiberadorSap_Id === Id)).map(Id => ({ LiberadorSap_Id: Id }));
                            this.model.liberadoresSap = this.model.liberadoresSap.concat(nuevos);
                            //cargo los select con todos los liberadores segun su cargo:
                            this.liberadoresJefes = listaResult.filter(j => j.Cargo === 'Jefe').map(j => ({ label: j.NombreCompleto, value: j.Id, disabled: j.Obligatorio }));
                            this.liberadoresGerentes = listaResult.filter(g => g.Cargo === 'Gerente').map(g => ({ label: g.NombreCompleto, value: g.Id, disabled: g.Obligatorio }));
                            this.liberadoresDirectores = listaResult.filter(d => d.Cargo === 'Director').map(d => ({ label: d.NombreCompleto, value: d.Id, disabled: d.Obligatorio }));
                            //tildo en los select los que ya trae la solp en su atributo liberadoresSap
                            var liberadoresIds = this.liberadoresJefes.map(x => x.value);
                            this.selectJefes = this.selectJefes.concat(this.model.liberadoresSap.map(lib => lib.LiberadorSap_Id)
                                .filter(id => !this.selectJefes.includes(id) && liberadoresIds.includes(id)));
                            liberadoresIds = this.liberadoresGerentes.map(x => x.value);
                            this.selectGerentes = this.selectGerentes.concat(this.model.liberadoresSap.map(lib => lib.LiberadorSap_Id)
                                .filter(id => !this.selectGerentes.includes(id) && liberadoresIds.includes(id)));
                            liberadoresIds = this.liberadoresDirectores.map(x => x.value);
                            this.selectDirectores = this.selectDirectores.concat(this.model.liberadoresSap.map(lib => lib.LiberadorSap_Id)
                                .filter(id => !this.selectDirectores.includes(id) && liberadoresIds.includes(id)));

                            this.validarChecks();
                        }
                    },
                    error => {
                        this.floatMsgService.setErrorMsg(error.message);
                    });
            } catch (e) {
                this.floatMsgService.setErrorMsg(e);
                return false;
            }
            return false;
        }
    }

    onLiberadoresChange(event) {
        const existingIndex = this.model.liberadoresSap.findIndex(lib => lib.LiberadorSap_Id === event.itemValue);
        if (existingIndex !== -1) {
            this.model.liberadoresSap.splice(existingIndex, 1);
        } else {
            this.model.liberadoresSap.push({ LiberadorSap_Id: event.itemValue });
        }
        this.validarChecks();
    }

    resetearFecha(): void {
        if (this.model.trabajoHecho != true || this.model.urgencia != true) {
            if (this.model.fechaEntrega < this.hoy) {
                this.model.fechaEntrega = this.hoy;
            }
        }
    }

    onRadioButtonChange(value) {
        this.model.thServicioPermanente = false;
        this.model.thAjustePolinomica = false;
        this.model.thProveedorDirecto = false;
        this.model.thAcuerdoMarco = false;

        if (value == "Servicio permanente") {
            this.model.thServicioPermanente = true;
        }

        if (value == "Ajuste polinomica") {
            this.model.thAjustePolinomica = true;
        }

        if (value == "Proveedor directo") {
            this.model.thProveedorDirecto = true;
        }

        if (value == "Acuerdo Marco") {
            this.model.thAcuerdoMarco = true;
        }
        // Evaluamos el mensaje a mostrar
        this.evaluarMostrarInfoBox();
    }

    habilitarOC() {
        if (this.model.editarCondicionesEspeciales == true) {
            this.formularioCotizacion.controls['ordenDeCompra'].enable();
        } else {
            this.formularioCotizacion.controls['ordenDeCompra'].disable();
        }
    }

    verificarCondicionesEspeciales(): void {
        if (!this.model.condEspProveedorAsignado && !this.model.urgencia && !this.model.adicional && !this.model.trabajoHecho && !this.model.conPresupuesto) {
            this.borrarArchivosCargados();
        }
        this.evaluarMostrarInfoBox();
    }

    borrarArchivosCargados(): void {
        if (this.model.editarCondicionesEspeciales) {
            this.model.archivosCotizacionesNuevosCondEsp = [];
            this.model.archivosCotizacionesCondEsp = [];

        }
    }

    habilitarCondicionesEspeciales(condicionEspecial) {
        if (!condicionEspecial && this.model.editarCondicionesEspeciales == true) {
            this.habilitarUrgencia(true);
            this.habilitarAdicional(true);
            this.habilitarTrabajoHecho(true);
            this.habilitarConPresupuesto(true);
            this.habilitarCertificacionAutomatica(true);
            this.habilitarCondEspProveedorAsignado(true);
            this.habilitarProveedorSeleccionado(true);
        }
    }

    private adjustFormControlsBasedOnConditions(): void {
        if (this.model.trabajoHecho && this.model.urgencia) {
            this.model.thAjustePolinomica = false;
            this.ajustePolinomicaDisabled = true;
        } else {
            this.ajustePolinomicaDisabled = false;
        }

        this.condicionEspecial = this.model.trabajoHecho == true || this.model.conPresupuesto || this.model.adicional == true || this.model.urgencia == true || this.model.condEspProveedorAsignado == true;
        this.habilitarCondicionesEspeciales(this.condicionEspecial)

        if (this.model.editarCondicionesEspeciales == false) {

            this.habilitarUrgencia(false);
            this.habilitarAdicional(false);
            this.habilitarTrabajoHecho(false);
            this.habilitarConPresupuesto(false);
            this.habilitarCertificacionAutomatica(false);
            this.habilitarProveedorSeleccionado(false);
            this.habilitarCondEspProveedorAsignado(false);

            if (this.condEspOriginales.trabajoHecho || (this.condEspOriginales.trabajoHecho && this.condEspOriginales.adicional)) {
                this.habilitarAdicional(true);
                this.habilitarUrgencia(true);
                this.verificarMismoProveedor();
            }
            else {
                if (this.condEspOriginales.proveedorAsignado || this.condEspOriginales.adicional) {

                    if (this.model.condEspProveedorAsignado == true) {
                        this.habilitarCondEspProveedorAsignado(true);
                        this.habilitarProveedorSeleccionado(true);
                    }

                    if (this.model.adicional == true) {
                        this.habilitarAdicional(true);
                        this.habilitarCondEspProveedorAsignado(false);
                        this.habilitarProveedorSeleccionado(false);
                    }

                    if (this.model.condEspProveedorAsignado != true && this.model.adicional != true) {
                        this.habilitarAdicional(true);
                        this.habilitarCondEspProveedorAsignado(true);
                        this.habilitarProveedorSeleccionado(true);
                    }
                    this.verificarMismoProveedor();

                }
                else {
                    if (this.condEspOriginales.urgencia) {
                        this.habilitarAdicional(false);
                        this.habilitarCondEspProveedorAsignado(false);
                        this.habilitarTrabajoHecho(false);
                        this.habilitarConPresupuesto(false);
                        this.habilitarCertificacionAutomatica(false);
                        this.habilitarProveedorSeleccionado(false);
                    }
                }
            }

            if (this.condicionEspecial != true) {
                this.validarCondicionEspecial()
            }
        }
        else {
            this.condicionesEspecialesSolpSinLiberar();
        }

        if (this.model.trabajoHecho) {
            this.habilitarConPresupuesto(false);
            this.habilitarCertificacionAutomatica(false);
        }
        if (this.model.conPresupuesto) {
            this.habilitarTrabajoHecho(false);
        }
    }

    condicionesEspecialesSolpSinLiberar() {
        if (this.model.trabajoHecho == true || this.model.adicional == true) {
            this.habilitarCondEspProveedorAsignado(false);
            this.habilitarProveedorSeleccionado(false);
        } else {
            this.habilitarCondEspProveedorAsignado(true);
            this.habilitarProveedorSeleccionado(true);
        }

        if (this.model.condEspProveedorAsignado == true) {
            this.habilitarTrabajoHecho(false);
            this.habilitarConPresupuesto(false);
            this.habilitarCertificacionAutomatica(false);
            this.habilitarAdicional(false);
        } else {
            this.habilitarTrabajoHecho(true);
            this.habilitarConPresupuesto(true);
            this.habilitarCertificacionAutomatica(true);
            this.habilitarProveedorSeleccionado(true);
            this.habilitarAdicional(true);
        }

        // Agregar esta nueva lógica para deshabilitar mutuamente "Con Presupuesto" y "Proveedor Asignado"
        if (this.model.conPresupuesto == true) {
            this.habilitarCondEspProveedorAsignado(false);
        } else if (this.model.condEspProveedorAsignado == true) {
            this.habilitarConPresupuesto(false);
        } else {
            // Si ninguno está marcado, habilitar ambos (siempre que no haya otras restricciones)
            if (!this.model.trabajoHecho && !this.model.adicional) {
                this.habilitarCondEspProveedorAsignado(true);
                this.habilitarConPresupuesto(true);
            }
        }
    }

    onConditionChange(): void {
        this.adjustFormControlsBasedOnConditions();
        if (this.condicionEspecial) {
            this.verificarMismoProveedor();
        }
        this.evaluarMostrarInfoBox();
    }

    verificarMismoProveedor() {
        if (this.ordenDeCompraSap != undefined && this.condEspOriginales.proveedorSeleccionado != this.ordenDeCompraSap.Cabecera.Usuario_Id) {
            this.floatMsgService.setErrorMsg("El proveedor seleccionado debe ser el mismo que el proveedor asignado");
        }
    }

    tieneCondEspOriginal() {
        return this.condicionEspecialOriginal = this.condEspOriginales.trabajoHecho == true || this.condEspOriginales.adicional == true || this.condEspOriginales.urgencia == true || this.condEspOriginales.proveedorAsignado == true;
    }

    validarCondicionEspecial() {
        if (this.tieneCondEspOriginal()) {
            this.floatMsgService.setErrorMsg("Debe completar la condicion especial");
        }
    }

    puedeSeleccionarPliegoMultiple(): boolean {
        const hayCondicionEspecialSeleccionada = this.model.trabajoHecho || this.model.conPresupuesto || this.model.adicional || this.model.urgencia || this.model.condEspProveedorAsignado;
        return !this.esUsuarioRRHH && !hayCondicionEspecialSeleccionada;
    }

    puedeSeleccionarCondicionEspecial(): boolean {
        return !this.model.seraUsadoEnPliegoMultiple;
    }

    habilitarUrgencia(habilitar: boolean) {
        this.habilitarControlFormulario('urgencia', habilitar);
    }

    habilitarAdicional(habilitar: boolean) {
        this.habilitarControlFormulario('adicional', habilitar);
    }

    habilitarTrabajoHecho(habilitar: boolean) {
        this.habilitarControlFormulario('trabajoHecho', habilitar);
    }

    habilitarConPresupuesto(habilitar: boolean) {
        this.habilitarControlFormulario('conPresupuesto', habilitar);
    }

    habilitarCertificacionAutomatica(habilitar: boolean) {
        this.habilitarControlFormulario('certificacionAutomatica', habilitar);
    }

    habilitarProveedorSeleccionado(habilitar: boolean) {
        this.habilitarControlFormulario('proveedorSeleccionado', habilitar);
    }

    habilitarCondEspProveedorAsignado(habilitar: boolean) {
        this.habilitarControlFormulario('condEspProveedorAsignado', habilitar);
    }

    habilitarControlFormulario(control: string, habilitar: boolean) {
        if (habilitar) {
            this.formularioCotizacion.controls[control].enable();
        }
        else {
            this.formularioCotizacion.controls[control].disable();
        }
    }

    evaluarMostrarInfoBox(): void {
        this.infoBoxText = "";
        this.mostrarInfoBox = false;

        let textos: string[] = [];

        // SOLP donde ya se tiene el precio (Si se marca Trabajo hecho o Con Presupuesto) 
        if (this.model.trabajoHecho == true && this.model.thServicioPermanente == true) {
            textos.push("Referencia la necesidad de un servicio ya realizado o material entregado, rutinario y que no queda bajo las condiciones de un Acuerdo Marco (AM). El fiscal podrá recibir desde compras el presupuesto, para la carga de la SOLP o de tener el fiscal la oferta procederá a cargarla.")
        }
        if (this.model.trabajoHecho == true && this.model.thAjustePolinomica == true) {
            textos.push("Referencia la necesidad de un ajuste por condiciones comerciales (Polinómica de ajuste) sobre un servicio ya realizado o producto entregado. El fiscal recibirá el control económico realizado desde compras, generará su propio control y gestionará la carga de la SOLP.");
        }
        if (this.model.trabajoHecho == true && this.model.thProveedorDirecto == true) {
            textos.push("Referencia la necesidad de un servicio ya realizado o material entregado, no rutinario. El fiscal define un proveedor directo técnicamente por sus condiciones especiales o particularidad del trabajo o producto.El fiscal podrá recibir desde compras el presupuesto para la carga de la SOLP o de tener la oferta procederá a cargar la misma.");
        }

        if (this.model.trabajoHecho == true && this.model.thAcuerdoMarco == true) {
            textos.push("Trabajo Realizado con acuerdo marco");
            textos.push("Referencia la necesidad de un servicio ya realizado o material entregado, rutinario que queda bajo las condiciones de un Acuerdo Marco (AM).");
        }

        if (this.model.conPresupuesto == true) {
            textos.push("Referencia la necesidad de un servicio a realizar o material a entregar, sin un proceso de licitación bajo las condiciones estándar. El fiscal podrá recibir desde compras el presupuesto para la carga de la SOLP o de tener el fiscal la oferta procederá a cargar la misma. La SOLP, con su alcance técnico, cantidad, precio unitario, justificaciones, etc., será vinculada a un proveedor definido. Podrán emitirse circulares comerciales cuando así corresponda, validadas estas se avanzará con la orden de compra.");
        }

        // SOLP donde hay que salir a buscar la Oferta
        if (this.model.condEspProveedorAsignado == true) {
            textos.push("Referencia la necesidad de un servicio a realizar o material a entregar.");
            textos.push("La solp será asignada a un proveedor definido técnicamente por sus condiciones especiales o particularidad del servicio, material, insumo o equipo.");
        }
        if (this.model.trabajoHecho == false && this.model.conPresupuesto == false) {
            textos.push("Generará un proceso de licitación bajo las condiciones estándar.");
            textos.push("El fiscal al momento de la carga de la SOLP solicitara un alcance técnico del servicio o producto y un precio a cotizar.");
        }

        if (this.model.adicional == true) {
            textos.push("Estas SOLPS nuevas se agregarán a una OC ya existente ya que hubo un proceso de licitación ya ejecutado y adjudicado con anterioridad.");
        }
        else {
            textos.push("Validadas las condiciones técnicas y comerciales cuando así lo requiera, esta SOLP nueva permitirá gestionar una OC puntual.")
        }
        if (this.model.urgencia == true) {
            textos.push("Referencia la necesidad de un servicio realizado o material entregado como urgente.");
        }

        // En caso hay tilde de certificación automática
        if (this.model.certificacionAutomatica == true) {
            textos.push("LA CERTIFICACION DEL SERVICIO SE HARA DE FORMA AUTOMATICA. EL PROVEEDOR QUEDA AUTORIZADO A COBRAR EL SERVICIO.");
        }
        if (this.model.certificacionAutomatica == false) {
            textos.push("VA A REQUERIR DEFINIR DESDE COMPRAS SI ADMITE O NO CERTIFICACIONES PARCIALES EN LOS SERVICIOS.");
        }

        // Si hay condiciones especiales, mostrar el info box
        if (textos.length > 0) {
            this.infoBoxText = textos.join("\n");
            this.mostrarInfoBox = true;
        } else {
            this.mostrarInfoBox = false;
        }
    }

    validarCondicionesDeAcuerdoMarco() {
        // Si no hay posiciones, no hay nada que validar
        if (this.model.posiciones.length === 0) {
            return false;
        }

        // Filtrar solo posiciones activas (estado == true)
        const posicionesActivas = this.model.posiciones.filter(x => x.estado === true);
        // Si no hay posiciones activas, no hay nada que validar
        if (posicionesActivas.length === 0) {
            return true;
        }

        // 1. Verificar si todas las posiciones activas tienen contrato marco o todas no tienen
        const tienenContratoMarco = posicionesActivas.filter(
            x => x.numeroContratoSuperior != null &&
                x.numeroContratoSuperior !== undefined &&
                x.numeroContratoSuperior !== ""
        );

        const sinContratoMarco = posicionesActivas.filter(
            x => x.numeroContratoSuperior === null ||
                x.numeroContratoSuperior === undefined ||
                x.numeroContratoSuperior === ""
        );

        // 2. Si hay posiciones con contrato marco y sin contrato marco al mismo tiempo,
        // la función debe devolver false (condición no permitida)
        if (tienenContratoMarco.length > 0 && sinContratoMarco.length > 0) {
            return false;
        }

        // 3. Si todas las posiciones activas tienen contrato marco, es válido
        // También es válido que posiciones activas tengan diferentes contratos marco
        if (tienenContratoMarco.length === posicionesActivas.length) {
            return true;
        }

        // 4. Si todas las posiciones activas NO tienen contrato marco, también es válido
        if (sinContratoMarco.length === posicionesActivas.length) {
            return true;
        }

        // Este punto no debería alcanzarse, pero por seguridad devolvemos false
        return false;
    }
};