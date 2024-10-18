import { Component, ViewChild, OnInit } from '@angular/core';
import { UsuarioService } from './../usuario.service';
import { BaseComponent } from './../../common/base-components/base-component';
import { Usuario } from './../usuario';
import { MensajeComponent } from './../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../../common/view-child/spinner/spinner.component';
import { DropdownComponent, DropdownOption } from './../../common/view-child/dropdown/dropdown.component';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { SecurityService } from './../../common/services/SecurityService';
import { SessionDataService } from './../../common/services/SessionDataService';
import { Seccion } from './../../common/models/seccion';
import { ModalService } from './../../common/services/ModalService';



@Component({
    selector: 'app-usuario-alta-contrasenia',
    templateUrl: `usuario.alta.component.html`,
    providers: [UsuarioService]
})
export class AltaUsuarioComponent extends BaseComponent implements OnInit {

    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    @ViewChild('dropdown_perfil')
    protected perfilDropdownComponent: DropdownComponent;

    @ViewChild('dropdown_tipo')
    protected tipoDropdownComponent: DropdownComponent;

    constructor(protected service: UsuarioService, protected navService: NavService, protected securityService: SecurityService, protected sessionDataService: SessionDataService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(navService, securityService, floatMsgService, modalService);
        this.mensajeComponent = new MensajeComponent();
        this.spinnerComponent = new SpinnerComponent();
        this.perfilDropdownComponent = new DropdownComponent();
        this.tipoDropdownComponent = new DropdownComponent();
    }

    numeroProveedor: string;
    usuarioId: string;
    tipo: string;
    email: string;
    tipoOptions: Array<DropdownOption> = [];
    perfilOptions: Array<DropdownOption> = [];
    perfiles: Array<string> = [];
    usuarios: Array<Usuario> = new Array<Usuario>();
    visibleButton: boolean = true;

    setTabs() {
        this.setMenuSeccionTab('usuario', 'Alta Usuario');
    }

    ngOnInit() {
        this.setTabs();
        this.securityService.tienePermisoRedirect("ABM USUARIOS");
        this.navService.setSeccionList([new Seccion('/usuario/list', 'usuario', 'Listado Usuarios'), new Seccion('/usuario/alta', 'usuario', 'Alta Usuario')]);
        this.getPerfilesOptions();
    }

    alta() {
        let usuario: Usuario = { numeroProveedor: this.numeroProveedor, email: this.email, perfil: this.perfilDropdownComponent.selectedOption, tipo: this.tipoDropdownComponent.selectedOption, Mail: "" };
        this.mensajeComponent.setMsgsEmpty();
        this.visibleButton = false;
        //this.usuarios.push(usuario); -> Soporta un solo usuario
        this.spinnerComponent.showIt();
        this.unsubscribe();
        try {
            this.subscription = this.service.alta(usuario).subscribe(
                (result:any) => {
                    this.visibleButton = true;
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.vaciarInputs();
                        this.mensajeComponent.setSuccessMsg(result.data);
                    }
                },
                error => {
                    this.visibleButton = true;
                    this.spinnerComponent.hideIt();
                    this.mensajeComponent.setErrorMsg(error.message);
                }

            );
        } catch (e) {
            this.visibleButton = true;
            this.spinnerComponent.hideIt();
            this.mensajeComponent.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }

        return false; //<-- Prevent Refresh
    }

    getPerfilesOptions() {
        try {
            this.subscriptionDropDowns = this.service.getPerfiles().subscribe(
                (result:any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.perfilOptions = result.data.perfiles;
                        this.tipoOptions = result.data.tipos;
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

    tienePerfiles() {
        return this.perfiles.length > 0;
    }

    vaciarInputs() {
        this.numeroProveedor = "";
        this.usuarioId = "";
        this.email = "";
        this.perfiles = [];
        this.usuarios = [];
    }
}