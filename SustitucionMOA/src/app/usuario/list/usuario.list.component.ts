import { Component, ElementRef, OnInit, ViewChild} from '@angular/core';
import { DropdownComponent } from '../../common/view-child/dropdown/dropdown.component';
import { BaseComponent } from './../../common/base-components/base-component';
import { Seccion } from './../../common/models/seccion';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { ModalService } from './../../common/services/ModalService';
import { NavService } from './../../common/services/NavService';
import { SecurityService } from './../../common/services/SecurityService';
import { SessionDataService } from './../../common/services/SessionDataService';
import { MensajeComponent } from './../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../../common/view-child/spinner/spinner.component';
import { UsuarioService } from './../usuario.service';
import { Rol } from '../../common/models/rol';
import { ModificarDatosComponent } from '../modificar-datos/modificar-datos.component';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Calendar } from 'primeng/calendar';
import { BasicResponse } from '../../common/models/response';

@Component({
    selector: 'app-usuario-list',
    templateUrl: `usuario.list.component.html`,
    styleUrls: [
        './usuario.list.component.css',
    ],
    providers: [UsuarioService]
})
export class UsuarioListComponent extends BaseComponent implements OnInit {
    formularioUsuario: FormGroup;
    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    @ViewChild('dropdown_rol')
    protected rolDropdownComponent: DropdownComponent;

    @ViewChild(ModificarDatosComponent)
    protected modificarDatosComponent: ModificarDatosComponent;

    @ViewChild('fechaReasignar') calendar: Calendar;

    @ViewChild('externoCheckbox') externoCheckbox!: ElementRef;

    
    form: FormGroup;

    constructor(protected service: UsuarioService, protected navService: NavService, protected securityService: SecurityService, protected sessionDataService: SessionDataService, 
        protected floatMsgService: FloatMsgService, protected modalService: ModalService, private fb: FormBuilder) {
        super(navService, securityService, floatMsgService, modalService);
        this.mensajeComponent = new MensajeComponent();
        this.spinnerComponent = new SpinnerComponent();
        this.rolDropdownComponent = new DropdownComponent();

        this.service.getUsuarioRecargarLista().subscribe(recargar =>{
            if (recargar!=null && recargar == true) this.getUsuario();
        });

        this.form = this.fb.group({
            suplente: ['']
          });
    }

    data: any;
    mailsUsuariosAprobadores: string[] = [];
    orderedByColumn: string = "id";
    orderDirection: number = 1;
    itemsPerPage = 20;
    filtroUsuarioVendedor: string = "";
    rolOptions: Array<Rol> = [];
    rolOptionsAll: Array<Rol> = [];
    rolesUsuarioSeleccionado: Array<Rol> = [];
    usuarioSeleccionado: any = {};
    titulos: Array<string> = ["Externo", "Interno", "Contacto"]
    usuarioModificacionSel: string = '';
    rangoReasignacion: Date[];
    fechaReasignacionMin: Date = new Date();
    fechaReasignacionMax: Date = new Date(new Date().setFullYear(new Date().getFullYear() + 100));
    es: any;
    validationError: boolean = false;
    beInfo: boolean = true;
    userChangedValue: boolean = false;
    esExterno: boolean = false; 
    isCheckboxDisabled: boolean = true;

    tienePermisoEditarSuplente: boolean = false;
    tienePermisoEditarUsuario: boolean = false;

    setTabs() {
        this.setMenuSeccionTab('usuario', 'Listado Usuarios');
    }

    ngOnInit() {
        this.verificarPermisosEdicion();

        this.es = {
            firstDayOfWeek: 0,
            dayNames: ["Domingo", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado"],
            dayNamesShort: ["Dom", "Lun", "Mar", "Mié", "Jue", "Vie", "Sáb"],
            dayNamesMin: ["Do", "Lu", "Ma", "Mi", "Ju", "Vi", "Sá"],
            monthNames: ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"],
            monthNamesShort: ["Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic"],
            today: 'Confirmar',
            clear: 'Limpiar',
            dateFormat: 'dd/mm/yyyy',
            weekHeader: 'Sem'
        };
        this.formularioUsuario = new FormGroup({
            usuarioSap: new FormControl('', [
                Validators.pattern(/^[A-Za-z]+(?:\s[A-Za-z]+)*$/)
            ]),
            suplente: new FormControl({ value: '', disabled: !this.tienePermisoEditarSuplente }, [
                Validators.pattern(/^\S+$/)
            ]),
            fechaReasignar1: new FormControl('', Validators.required)
        });
        
        this.setTabs();
        this.securityService.tieneAlgunPermisoRedirect(["ABM USUARIOS", "EDITAR SUPLENTE"]);
        this.navService.setSeccionList([new Seccion('/usuario/list', 'usuario', 'Listado Usuarios')]);
        //this.navService.setSeccionList([new Seccion('/usuario/list', 'usuario', 'Listado Usuarios'), new Seccion('/usuario/alta', 'usuario', 'Alta Usuario')]);
        this.getUsuario();
        this.getRolesOptions();
        this.getMailsUsuariosAprobadores();

        this.formularioUsuario = this.fb.group({
            usuarioSap: ['', [Validators.required]],
            suplente: ['', [Validators.required]],
            fechaReasignar1: [{ value: null, disabled: true }, [Validators.required]]
          });
      
        this.formularioUsuario.get('suplente').valueChanges.subscribe(value => {
            if (this.tienePermisoEditarSuplente) {
                if (value) {
                    this.formularioUsuario.get('fechaReasignar1').enable();
                } else {
                    this.formularioUsuario.get('fechaReasignar1').disable();
                    this.formularioUsuario.get('fechaReasignar1').reset();
                }
            } else {
                this.formularioUsuario.get('fechaReasignar1').disable();
            }
        });
    }

    verificarPermisosEdicion() {
        this.tienePermisoEditarUsuario = this.securityService.tienePermiso("ABM USUARIOS");
        this.tienePermisoEditarSuplente = this.securityService.tienePermiso("EDITAR SUPLENTE");
    }

    getRolesOptions() {
        try {
            this.subscriptionDropDowns = this.service.getRoles().subscribe(
                (result:any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.rolOptionsAll = result.data.roles;
                    }
                },
                error => {
                    this.mensajeComponent.setErrorMsg(error.message);
                }

            );
        } catch (e) {
            this.mensajeComponent.setErrorMsg(e);
        }
    }

    abrirModalAuditoriaUsuario(id: number){
        this.service.setUsuarioCargarAuditoria(id);
    }

    abrirModalModificarDatos(usuario){
        const id:number = usuario.Id;
        this.usuarioModificacionSel = usuario.Mail;
        this.service.setUsuarioModificarDatos(id);
    }

    cerrarModalModificarDatos(event){
        if(event){
            let modal = document.getElementById('cerrarModalUsuario');
            modal.click();
        }
    }

    getUsuario() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        try {
            this.unsubscribe();
            this.subscription = this.service.getUsuarios().subscribe(
                (result:any) => {
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.data = result.data.usuarios;
                    }
                    
                },
                error => {
                    this.spinnerComponent.hideIt();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
        } catch (e) {
            this.spinnerComponent.hideIt();
            this.mensajeComponent.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    }

    getMailsUsuariosAprobadores() {
        this.subscription = this.service.getMailsUsuariosAprobadores().subscribe(
            (response) => {
                let mailsAprobadores = this.manejarErroresApiResponse(response);
                if (mailsAprobadores) {
                    this.mailsUsuariosAprobadores = mailsAprobadores;
                }
            },
            error => {
                this.mensajeComponent.setErrorMsg(error);
            }
        );
    }

    isVisible() {
        return this.data && this.data.length != 0;
    }

    orderColumnBy(column: string) {
        if (column == this.orderedByColumn) {
            this.orderDirection = -this.orderDirection;
        } else {
            this.orderDirection = 1;
            this.orderedByColumn = column;
        }
    }

    desbloquear(usuario: string) {
        this.spinnerComponent.showIt();
        this.mensajeComponent.setMsgsEmpty();
        try {
            this.service.desbloquearUsuario(usuario).subscribe(
                (result:any) => {
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.mensajeComponent.setSuccessMsg(result.data);
                        this.getUsuario();
                    }
                },
                error => {
                    this.mensajeComponent.setErrorMsg(error.message);
                }

            );
        } catch (e) {
            this.spinnerComponent.hideIt();
            this.mensajeComponent.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    }

    deshabilitar(mailUsuario: string) {
        this.spinnerComponent.showIt();
        this.mensajeComponent.setMsgsEmpty();
        try {
            this.service.deshabilitarUsuario(mailUsuario).subscribe(
                (result:any) => {
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.mensajeComponent.setSuccessMsg(result.data);
                        this.getUsuario();
                    }
                },
                error => {
                    this.mensajeComponent.setErrorMsg(error.message);
                }

            );
        } catch (e) {
            this.spinnerComponent.hideIt();
            this.mensajeComponent.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    }

    habilitar(mailUsuario: string) {
        this.spinnerComponent.showIt();
        this.mensajeComponent.setMsgsEmpty();
        try {
            this.service.habilitarUsuario(mailUsuario).subscribe(
                (result:any) => {
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.mensajeComponent.setSuccessMsg(result.data);
                        this.getUsuario();
                    }
                },
                error => {
                    this.mensajeComponent.setErrorMsg(error.message);
                }

            );
        } catch (e) {
            this.spinnerComponent.hideIt();
            this.mensajeComponent.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    }

    isBloqueado(bloqueado: string) {
        return bloqueado == "X";
    }

    isHabilitado(estado: string) {
        return estado == "H";
    }

    showModalTableResponsive(usuario: any) {
        this.modalService.openModalTableResponsive("Usuario", [
            { etiqueta: "ID", valor: usuario.id },
            { etiqueta: "Usuario", valor: usuario.usuario },
            { etiqueta: "Vendedor", valor: usuario.vendedor }
        ]);
        return false;
    }

    abrirModalEditarRoles(usuario: any) {
        this.usuarioSeleccionado = usuario;
        this.formularioUsuario.controls['usuarioSap'].patchValue(usuario.UsuarioSap);
        // this.formularioUsuario.controls['suplente'].patchValue(usuario.Suplente);

        // if (usuario.Suplente) {
        //     this.isCheckboxDisabled = false;
        // }
        // this.cleanReasignarInput();
        this.rolesUsuarioSeleccionado = new Array<Rol>();
        this.rolOptions = [];
        this.rolOptionsAll.forEach(val => this.rolesUsuarioSeleccionado.push(Object.assign({}, val)));

        for (var i = 0; i < this.rolesUsuarioSeleccionado.length; i++) {
            this.rolesUsuarioSeleccionado[i].checked = false;
        }

        // this.externoCheckbox.nativeElement.checked = usuario.Externo;

        usuario.Roles = this.obtenerRolesUsuario();
        this.obtenerReasignacionUsuario();
/*
        usuario.Roles.forEach(element => {
            let index = this.rolesUsuarioSeleccionado.findIndex(r => r.Id.toString() == element.Id.toString());

            if (index > -1)
                this.rolesUsuarioSeleccionado[index].checked = true;
        });*/

        return false;
    }

    abrirModalEditarSuplente(usuario: any) {
        this.usuarioSeleccionado = usuario;
        this.formularioUsuario.controls['suplente'].patchValue(usuario.Suplente);

        if (usuario.Suplente) {
            this.isCheckboxDisabled = false;
        }
        this.externoCheckbox.nativeElement.checked = usuario.Externo;

        document.getElementById("openModalSuplenteHiddenButton").click();
        this.obtenerReasignacionUsuario();
        return false;
    }

    obtenerRolesUsuario() {
         try {
            this.service.obtenerRolesUsuario(this.usuarioSeleccionado).subscribe(
                (result:any) => {
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {

                        result.data.forEach(element => {
                            let index = this.rolesUsuarioSeleccionado.findIndex(r => r.Id.toString() == element.Id.toString());

                            if (index > -1)
                                this.rolesUsuarioSeleccionado[index].checked = true;
                        });
                        document.getElementById("openModalHiddenButton").click();
                    }
                },
                error => {
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
        } catch (e) {
            this.spinnerComponent.hideIt();
            this.mensajeComponent.setErrorMsg(e);
        }
    }

    obtenerReasignacionUsuario() {
        try {
            this.service.obtenerReasignacionUsuario(this.usuarioSeleccionado).subscribe(
                (result: any) => {
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                        this.beInfo = false;
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                        this.beInfo = false;
                    } else {
                        if (result.data.FechaDesde !== '' && result.data.FechaHasta !== '' && result.data.Id !== 0) {
                            setTimeout(() => {
                                this.rangoReasignacion = [];
                                let dateString = result.data.FechaDesde.toString();
                                let ts = parseInt(dateString.match(/\d+/)[0], 10);
                                let jsonDate = new Date(ts);
                                this.rangoReasignacion[0] = jsonDate;

                                dateString = result.data.FechaHasta.toString();
                                ts = parseInt(dateString.match(/\d+/)[0], 10);
                                jsonDate = new Date(ts);
                                this.rangoReasignacion[1] = jsonDate;
                                this.beInfo = false;
                            }, 500);
                        }
                        else {
                            this.beInfo = false;
                        }
                    }
                },
                error => {
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
        } catch (e) {
            this.spinnerComponent.hideIt();
            this.mensajeComponent.setErrorMsg(e);
        }
    }

    onExternoChange(event: Event): void {
        this.esExterno = (event.target as HTMLInputElement).checked;
    }

    guardarSuplente() {
        if (this.validateReasignacionValues()) {
            const suplente = this.formularioUsuario.controls['suplente'].value;
            const usuarioId = this.usuarioSeleccionado.Id;
            
            this.spinnerComponent.showIt();
            this.mensajeComponent.setMsgsEmpty();

            let fDesde = '';
            let fHasta = '';
            if (this.rangoReasignacion !== null && this.rangoReasignacion.length > 0 && this.userChangedValue) {
                fDesde = this.dateFormatter(this.rangoReasignacion[0]);
                fHasta = this.dateFormatter(this.rangoReasignacion[1]);
            }

            this.service.guardarSuplente(usuarioId, suplente, fDesde, fHasta, this.esExterno).subscribe(
                (result: any) => {
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    }
                    else {
                        if (result.error != undefined && result.error != "") {
                            this.mensajeComponent.setErrorMsg(result.error);
                            document.getElementById("closeModalSuplente").click();
                        }
                        else {
                            if (result.info != undefined) {
                                this.mensajeComponent.setInfoMsg(result.info);
                            }
                            this.getUsuario();
                            document.getElementById("closeModalSuplente").click();
                            this.mensajeComponent.setSuccessMsg("El usuario ha sido actualizado correctamente");
                        }
                    }
                },
                error => {
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
            return false;
        }
    }

    guardarRolesUsuario() {
        const suplente = this.formularioUsuario.controls['suplente'].value;
        const usuarioSap = this.formularioUsuario.controls['usuarioSap'].value;
        
        this.usuarioSeleccionado.Suplente = suplente;
        this.usuarioSeleccionado.UsuarioSap = usuarioSap;
        this.spinnerComponent.showIt();
        this.mensajeComponent.setMsgsEmpty();
        const idRoles = this.rolesUsuarioSeleccionado.filter(r => r.checked).map(({ Id }) => Id);

        this.service.guardarRolesUsuario(this.usuarioSeleccionado, idRoles).subscribe(
            (result: any) => {
                this.spinnerComponent.hideIt();
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {
                    this.mensajeComponent.setSuccessMsg(result.data);
                    this.getUsuario();

                    document.getElementById("closeModal").click();
                    this.mensajeComponent.setSuccessMsg(result.data);
                }
            },
            error => {
                this.mensajeComponent.setErrorMsg(error.message);
            }
        );
        return false; //<-- Prevent Refresh
    }
    
    /**
     * Valida por expresiones regulares según el campo del formulario que se este utilizando.
     * @param controlName 
     */
    validarConExpresionesRegulares(controlName: string): void {
        const control = this.formularioUsuario.get(controlName);
        if (control) {
            switch (controlName) {
                // case 'suplente':
                //     control.setValue(control.value.replace(/\s+/g, ''));
                //     break;
                case 'usuarioSap':
                    let value = control.value
                    value = value.replace(/\s+/g, ' ');
                    control.setValue(value.toUpperCase());
                    break;
            }
        }
    }

    validarSuplente() {
        const controlSuplente = this.formularioUsuario.get('suplente');
        if (controlSuplente) {
            controlSuplente.setValue(controlSuplente.value.replace(/\s+/g, ''));
        }
    }

    validateReasignacionValues() {
        if (this.rangoReasignacion !== null && this.rangoReasignacion[1] === null) {
            this.validationError = true;
            return false;
        }
        this.validationError = false;
        return true;
    }

    dateFormatter(date_Object: Date): string {
        if (date_Object !== undefined) {
            const year = date_Object.getFullYear();
            const month = (date_Object.getMonth() + 1 < 10 ? '0' : '') + (date_Object.getMonth() + 1);
            const day = (date_Object.getDate() < 10 ? '0' : '') + date_Object.getDate();

            const date_String: string = `${year}-${month}-${day}`;
            return date_String;
        }
        return '';
    }

    limpiarCalendarioAsignacionSuplente() {
        //this.cleanReasignarInput();
        this.formularioUsuario.controls['fechaReasignar1'].setValue(null);
        this.formularioUsuario.controls['fechaReasignar1'].markAsPristine();
        this.formularioUsuario.controls['fechaReasignar1'].markAsUntouched();
        this.formularioUsuario.controls['fechaReasignar1'].updateValueAndValidity();
        this.validationError = false;
    }

    onCalendarChange(event: Event): void { 
        if (this.beInfo) {
            return;
        }
        else {
            this.userChangedValue = true;
        }
    }

    dontAllowCertificationRol(event: any): void {
        const certServ: string = 'CERTIFICACIÓN DE SERVICIOS';
        const certExt: string = 'CERTIFICACIÓN DE SERVICIOS EXT';

        if(event.target.value === certServ){
            this.rolesUsuarioSeleccionado.filter(r => r.Nombre === certExt).forEach(r => {
                if(r.checked){
                    r.checked = false;
                    document.getElementById("rol_" + certExt).click();
                }
            })
        }

        if(event.target.value === certExt){
            this.rolesUsuarioSeleccionado.filter(r => r.Nombre === certServ).forEach(r => {
                if(r.checked){
                    r.checked = false;
                    document.getElementById("rol_" + certServ).click();
                }
            })
        }
    }

    get suplenteControl(): FormControl {
        return this.formularioUsuario.controls['suplente'].value as FormControl;
    }

    onCalendarioAceptar() {
        // this.closeCalendar();
        this.calendar.overlayVisible = false;
        this.validationError = false;
    }

    suplentesFiltrados: string[] = [];
    suplenteEsValido: boolean = false;

    onInputSuplente() {
        var inputValue = this.formularioUsuario.controls['suplente'].value || '';
        if (inputValue.length > 3) {
            this.suplentesFiltrados = this.mailsUsuariosAprobadores.filter(x => x.toLowerCase().includes(inputValue));
            if (this.suplentesFiltrados[0] === inputValue) {
                this.suplentesFiltrados = [];
                this.isCheckboxDisabled = false;
            }
        }
        else {
            this.suplentesFiltrados = [];
            this.esExterno = false;
            this.externoCheckbox.nativeElement.checked = false;
        }
        this.isCheckboxDisabled = inputValue.trim() === '';
    }
    
    seleccionarSuplente(item: string) {
        this.formularioUsuario.controls['suplente'].setValue(item);
        this.formularioUsuario.controls['suplente'].markAsTouched();
        this.formularioUsuario.controls['suplente'].markAsDirty();
        this.suplentesFiltrados = [];

        this.isCheckboxDisabled = false;
    }

    validarEmailSuplente() {
        const inputValue = this.formularioUsuario.controls['suplente'].value;
        const esSuplenteValido = this.mailsUsuariosAprobadores.some(item => item === inputValue);

        if (!esSuplenteValido && inputValue.length > 0) {
            this.formularioUsuario.controls['suplente'].setErrors({ invalidEmail: true });
            this.suplenteEsValido = false;
            this.isCheckboxDisabled = true;
            this.esExterno = false;
            this.externoCheckbox.nativeElement.checked = false;
        }
        else {
            this.formularioUsuario.controls['suplente'].setErrors(null);
            this.suplenteEsValido = false;
            this.isCheckboxDisabled = false;      
        }
    }
      
    shouldShowErrorSuplente(): boolean {
        const control = this.formularioUsuario.controls['suplente'];

        if (control.touched && control.invalid && control.errors && control.errors.invalidEmail) {
            this.suplenteEsValido = true;
        }
        else {
            this.suplenteEsValido = false;
        }
        return control.touched && control.invalid && control.errors && control.errors.invalidEmail;
    }

    abrirModalVerVendedores(usuario) {
        this.usuarioModificacionSel = usuario.Mail;
        const id: number = usuario.Id;
        this.service.setUsuarioVerVendedores(id);
    }

    manejarErroresApiResponse<T>(response: BasicResponse<T>): T | undefined {
        if (response.logout) {
            this.sessionDataService.logout();
            return undefined;
        }
        if (response.error) {
            this.mensajeComponent.setErrorMsg(response.error);
            return undefined;
        }
        if (response.info) {
            this.mensajeComponent.setInfoMsg(response.info);
        }
        return response as T;
    }
}