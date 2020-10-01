var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
import { Component, ViewChild } from '@angular/core';
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
var UsuarioListComponent = /** @class */ (function (_super) {
    __extends(UsuarioListComponent, _super);
    function UsuarioListComponent(service, navService, securityService, sessionDataService, floatMsgService, modalService) {
        var _this = _super.call(this, navService, securityService, floatMsgService, modalService) || this;
        _this.service = service;
        _this.navService = navService;
        _this.securityService = securityService;
        _this.sessionDataService = sessionDataService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.orderedByColumn = "id";
        _this.orderDirection = 1;
        _this.itemsPerPage = 20;
        _this.filtroUsuarioVendedor = "";
        _this.rolOptions = [];
        _this.rolOptionsAll = [];
        _this.rolesUsuarioSeleccionado = [];
        _this.usuarioSeleccionado = null;
        _this.mensajeComponent = new MensajeComponent();
        _this.spinnerComponent = new SpinnerComponent();
        _this.rolDropdownComponent = new DropdownComponent();
        return _this;
    }
    UsuarioListComponent.prototype.setTabs = function () {
        this.setMenuSeccionTab('usuario', 'Listado Usuarios');
    };
    UsuarioListComponent.prototype.ngOnInit = function () {
        this.setTabs();
        this.securityService.tienePermisoRedirect("ABM USUARIOS");
        this.navService.setSeccionList([new Seccion('/usuario/list', 'usuario', 'Listado Usuarios')]);
        //this.navService.setSeccionList([new Seccion('/usuario/list', 'usuario', 'Listado Usuarios'), new Seccion('/usuario/alta', 'usuario', 'Alta Usuario')]);
        this.getUsuario();
        this.getRolesOptions();
    };
    UsuarioListComponent.prototype.getRolesOptions = function () {
        var _this = this;
        try {
            this.subscriptionDropDowns = this.service.getRoles().subscribe(function (result) {
                if (result.logout == true) {
                    _this.sessionDataService.logout();
                }
                else if (result.error != undefined && result.error != "") {
                    _this.mensajeComponent.setErrorMsg(result.error);
                }
                else if (result.info != undefined) {
                    _this.mensajeComponent.setInfoMsg(result.info);
                }
                else {
                    _this.rolOptionsAll = result.data.roles;
                }
            }, function (error) {
                _this.mensajeComponent.setErrorMsg(error.message);
            });
        }
        catch (e) {
            this.mensajeComponent.setErrorMsg(e);
        }
    };
    UsuarioListComponent.prototype.getUsuario = function () {
        var _this = this;
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        try {
            this.unsubscribe();
            this.subscription = this.service.getUsuarios().subscribe(function (result) {
                _this.spinnerComponent.hideIt();
                if (result.logout == true) {
                    _this.sessionDataService.logout();
                }
                else if (result.error != undefined && result.error != "") {
                    _this.mensajeComponent.setErrorMsg(result.error);
                }
                else if (result.info != undefined) {
                    _this.mensajeComponent.setInfoMsg(result.info);
                }
                else {
                    _this.data = result.data.usuarios;
                }
            }, function (error) {
                _this.spinnerComponent.hideIt();
                _this.mensajeComponent.setErrorMsg(error.message);
            });
        }
        catch (e) {
            this.spinnerComponent.hideIt();
            this.mensajeComponent.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    };
    UsuarioListComponent.prototype.isVisible = function () {
        return this.data && this.data.length != 0;
    };
    UsuarioListComponent.prototype.orderColumnBy = function (column) {
        if (column == this.orderedByColumn) {
            this.orderDirection = -this.orderDirection;
        }
        else {
            this.orderDirection = 1;
            this.orderedByColumn = column;
        }
    };
    UsuarioListComponent.prototype.desbloquear = function (usuario) {
        var _this = this;
        this.spinnerComponent.showIt();
        this.mensajeComponent.setMsgsEmpty();
        try {
            this.service.desbloquearUsuario(usuario).subscribe(function (result) {
                _this.spinnerComponent.hideIt();
                if (result.logout == true) {
                    _this.sessionDataService.logout();
                }
                else if (result.error != undefined && result.error != "") {
                    _this.mensajeComponent.setErrorMsg(result.error);
                }
                else if (result.info != undefined) {
                    _this.mensajeComponent.setInfoMsg(result.info);
                }
                else {
                    _this.mensajeComponent.setSuccessMsg(result.data);
                    _this.getUsuario();
                }
            }, function (error) {
                _this.mensajeComponent.setErrorMsg(error.message);
            });
        }
        catch (e) {
            this.spinnerComponent.hideIt();
            this.mensajeComponent.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    };
    UsuarioListComponent.prototype.deshabilitar = function (mailUsuario) {
        var _this = this;
        this.spinnerComponent.showIt();
        this.mensajeComponent.setMsgsEmpty();
        try {
            this.service.deshabilitarUsuario(mailUsuario).subscribe(function (result) {
                _this.spinnerComponent.hideIt();
                if (result.logout == true) {
                    _this.sessionDataService.logout();
                }
                else if (result.error != undefined && result.error != "") {
                    _this.mensajeComponent.setErrorMsg(result.error);
                }
                else if (result.info != undefined) {
                    _this.mensajeComponent.setInfoMsg(result.info);
                }
                else {
                    _this.mensajeComponent.setSuccessMsg(result.data);
                    _this.getUsuario();
                }
            }, function (error) {
                _this.mensajeComponent.setErrorMsg(error.message);
            });
        }
        catch (e) {
            this.spinnerComponent.hideIt();
            this.mensajeComponent.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    };
    UsuarioListComponent.prototype.habilitar = function (mailUsuario) {
        var _this = this;
        this.spinnerComponent.showIt();
        this.mensajeComponent.setMsgsEmpty();
        try {
            this.service.habilitarUsuario(mailUsuario).subscribe(function (result) {
                _this.spinnerComponent.hideIt();
                if (result.logout == true) {
                    _this.sessionDataService.logout();
                }
                else if (result.error != undefined && result.error != "") {
                    _this.mensajeComponent.setErrorMsg(result.error);
                }
                else if (result.info != undefined) {
                    _this.mensajeComponent.setInfoMsg(result.info);
                }
                else {
                    _this.mensajeComponent.setSuccessMsg(result.data);
                    _this.getUsuario();
                }
            }, function (error) {
                _this.mensajeComponent.setErrorMsg(error.message);
            });
        }
        catch (e) {
            this.spinnerComponent.hideIt();
            this.mensajeComponent.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    };
    UsuarioListComponent.prototype.isBloqueado = function (bloqueado) {
        return bloqueado == "X";
    };
    UsuarioListComponent.prototype.isHabilitado = function (estado) {
        return estado == "H";
    };
    UsuarioListComponent.prototype.showModalTableResponsive = function (usuario) {
        this.modalService.openModalTableResponsive("Usuario", [
            { etiqueta: "ID", valor: usuario.id },
            { etiqueta: "Usuario", valor: usuario.usuario },
            { etiqueta: "Vendedor", valor: usuario.vendedor }
        ]);
        return false;
    };
    UsuarioListComponent.prototype.abrirModalEditarRoles = function (usuario) {
        var _this = this;
        this.usuarioSeleccionado = usuario;
        this.rolesUsuarioSeleccionado = new Array();
        this.rolOptions = [];
        this.rolOptionsAll.forEach(function (val) { return _this.rolesUsuarioSeleccionado.push(Object.assign({}, val)); });
        for (var i = 0; i < this.rolesUsuarioSeleccionado.length; i++) {
            this.rolesUsuarioSeleccionado[i].checked = false;
        }
        usuario.Roles.forEach(function (element) {
            var index = _this.rolesUsuarioSeleccionado.findIndex(function (r) { return r.Id.toString() == element.Id.toString(); });
            _this.rolesUsuarioSeleccionado[index].checked = true;
        });
        document.getElementById("openModalHiddenButton").click();
        return false;
    };
    UsuarioListComponent.prototype.guardarRolesUsuario = function () {
        var _this = this;
        this.spinnerComponent.showIt();
        this.mensajeComponent.setMsgsEmpty();
        var idRoles = this.rolesUsuarioSeleccionado.filter(function (r) { return r.checked; }).map(function (_a) {
            var Id = _a.Id;
            return Id;
        });
        try {
            this.service.guardarRolesUsuario(this.usuarioSeleccionado, idRoles).subscribe(function (result) {
                _this.spinnerComponent.hideIt();
                if (result.logout == true) {
                    _this.sessionDataService.logout();
                }
                else if (result.error != undefined && result.error != "") {
                    _this.mensajeComponent.setErrorMsg(result.error);
                }
                else if (result.info != undefined) {
                    _this.mensajeComponent.setInfoMsg(result.info);
                }
                else {
                    _this.mensajeComponent.setSuccessMsg(result.data);
                    _this.getUsuario();
                    document.getElementById("closeModal").click();
                }
            }, function (error) {
                _this.mensajeComponent.setErrorMsg(error.message);
            });
        }
        catch (e) {
            this.spinnerComponent.hideIt();
            this.mensajeComponent.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    };
    __decorate([
        ViewChild(MensajeComponent),
        __metadata("design:type", MensajeComponent)
    ], UsuarioListComponent.prototype, "mensajeComponent", void 0);
    __decorate([
        ViewChild(SpinnerComponent),
        __metadata("design:type", SpinnerComponent)
    ], UsuarioListComponent.prototype, "spinnerComponent", void 0);
    __decorate([
        ViewChild('dropdown_rol'),
        __metadata("design:type", DropdownComponent)
    ], UsuarioListComponent.prototype, "rolDropdownComponent", void 0);
    UsuarioListComponent = __decorate([
        Component({
            selector: 'app-usuario-list',
            templateUrl: "usuario.list.component.html",
            providers: [UsuarioService]
        }),
        __metadata("design:paramtypes", [UsuarioService, NavService, SecurityService, SessionDataService, FloatMsgService, ModalService])
    ], UsuarioListComponent);
    return UsuarioListComponent;
}(BaseComponent));
export { UsuarioListComponent };
//# sourceMappingURL=usuario.list.component.js.map