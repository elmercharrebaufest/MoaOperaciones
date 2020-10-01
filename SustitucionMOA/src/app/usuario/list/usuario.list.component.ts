import { Component, OnInit, ViewChild } from '@angular/core';
import { DropdownOption, DropdownComponent } from '../../common/view-child/dropdown/dropdown.component';
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



@Component({
    selector: 'app-usuario-list',
    templateUrl: `usuario.list.component.html`,
    providers: [UsuarioService]
})
export class UsuarioListComponent extends BaseComponent implements OnInit {

    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    @ViewChild('dropdown_rol')
    protected rolDropdownComponent: DropdownComponent;

    constructor(protected service: UsuarioService, protected navService: NavService, protected securityService: SecurityService, protected sessionDataService: SessionDataService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(navService, securityService, floatMsgService, modalService);
        this.mensajeComponent = new MensajeComponent();
        this.spinnerComponent = new SpinnerComponent();
        this.rolDropdownComponent = new DropdownComponent();
    }

    data: any;
    orderedByColumn: string = "id";
    orderDirection: number = 1;
    itemsPerPage = 20;
    filtroUsuarioVendedor: string = "";

    rolOptions: Array<Rol> = [];
    rolOptionsAll: Array<Rol> = [];

    rolesUsuarioSeleccionado: Array<Rol> = [];

    usuarioSeleccionado: any = null;

    setTabs() {
        this.setMenuSeccionTab('usuario', 'Listado Usuarios');
    }

    ngOnInit() {
        this.setTabs();
        this.securityService.tienePermisoRedirect("ABM USUARIOS");
        this.navService.setSeccionList([new Seccion('/usuario/list', 'usuario', 'Listado Usuarios')]);
        //this.navService.setSeccionList([new Seccion('/usuario/list', 'usuario', 'Listado Usuarios'), new Seccion('/usuario/alta', 'usuario', 'Alta Usuario')]);
        this.getUsuario();
        this.getRolesOptions();
    }

    getRolesOptions() {
        try {
            this.subscriptionDropDowns = this.service.getRoles().subscribe(
                result => {
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

    getUsuario() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        try {
            this.unsubscribe();
            this.subscription = this.service.getUsuarios().subscribe(
                result => {
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
                result => {
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
                result => {
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
                result => {
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
        this.rolesUsuarioSeleccionado = new Array<Rol>();

        this.rolOptions = [];
        this.rolOptionsAll.forEach(val => this.rolesUsuarioSeleccionado.push(Object.assign({}, val)));

        for (var i = 0; i < this.rolesUsuarioSeleccionado.length; i++) {
            this.rolesUsuarioSeleccionado[i].checked = false;
        }

        usuario.Roles.forEach(element => {
            let index = this.rolesUsuarioSeleccionado.findIndex(r => r.Id.toString() == element.Id.toString());

            this.rolesUsuarioSeleccionado[index].checked = true;
        });

        document.getElementById("openModalHiddenButton").click();
        return false;
    }

    guardarRolesUsuario() {
        this.spinnerComponent.showIt();
        this.mensajeComponent.setMsgsEmpty();
        const idRoles = this.rolesUsuarioSeleccionado.filter(r => r.checked).map(({ Id }) => Id);
    
        try {
            this.service.guardarRolesUsuario(this.usuarioSeleccionado, idRoles).subscribe(
                result => {
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
    /*
    agregarRolUsuario() {
        var rolAgregar: Rol = new Rol();

        rolAgregar.Id = this.rolDropdownComponent.selectedOption,
        rolAgregar.Nombre = this.rolDropdownComponent.selectedOptionLabel;

        this.rolDropdownComponent.setInitial();
        this.rolesUsuarioSeleccionado.push(rolAgregar);

        //this.rolOptions[this.rolOptions.findIndex(r => r.value.toString() == rolAgregar.Id.toString())];

    }

    quitarRolUsuario(rol: Rol) {

        let index = this.rolesUsuarioSeleccionado.findIndex(r => r.Id.toString() == rol.Id.toString());

        this.rolesUsuarioSeleccionado.splice(index, 1);


        //delete this.rolesUsuarioSeleccionado[index];
    }*/
}