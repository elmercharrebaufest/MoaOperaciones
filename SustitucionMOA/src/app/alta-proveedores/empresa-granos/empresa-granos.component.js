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
import { Component, ViewChild } from "@angular/core";
import { FormControl } from "@angular/forms";
import { ActivatedRoute } from "@angular/router";
import { ListBaseComponent } from "../../common/base-components/list-base-component";
import { Archivo } from "../../common/models/archivo";
import { InformeComercial } from "../../common/models/informeComercial";
import { Material } from "../../common/models/material";
import { NuevoAcopio } from "../../common/models/nuevoAcopio";
import { NuevoProduccion } from "../../common/models/nuevoProduccion";
import { FloatMsgService } from "../../common/services/FloatMsgService";
import { ModalService } from "../../common/services/ModalService";
import { NavService } from "../../common/services/NavService";
import { SecurityService } from "../../common/services/SecurityService";
import { SessionDataService } from "../../common/services/SessionDataService";
import { MensajeComponent } from "../../common/view-child/mensaje/mensaje.component";
import { SpinnerSmallComponent } from "../../common/view-child/spinner-small/spinner-small.component";
import { EmpresaGranosService } from "./empresa-granos.service";
import { RelacionConEmpleados } from "../../common/models//RelacionConEmpleados";
import { RelacionConFuncionarios } from "../../common/models/relacionConFuncionarios";
import { CartaPresentacion } from "../../common/models/cartaPresentacion";
var EmpresaGranosComponent = /** @class */ (function (_super) {
    __extends(EmpresaGranosComponent, _super);
    function EmpresaGranosComponent(service, navService, route, sessionDataService, securityService, floatMsgService, modalService) {
        var _this = _super.call(this, service, navService, sessionDataService, securityService, floatMsgService, modalService) || this;
        _this.service = service;
        _this.navService = navService;
        _this.route = route;
        _this.sessionDataService = sessionDataService;
        _this.securityService = securityService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.listaMateriales = [];
        _this.listaCampanias = [];
        _this.empleados = [];
        _this.funcionarios = [];
        // private acopiosArray: Array<NuevoAcopio> = [];
        _this.newAttributeEmpleados = new RelacionConEmpleados();
        _this.newAttributeFuncionarios = new RelacionConFuncionarios();
        _this.newAttributeAlm = new NuevoAcopio();
        _this.newAttribute = new NuevoProduccion();
        _this.relacionConEmpleadosChecked = null;
        _this.relacionConFuncionariosChecked = null;
        _this.codigoConductaVisto = false;
        _this.codigoDeConducta = false;
        _this.materialesData = null;
        _this.CBUSISA = "";
        _this.proveedorCUIT = "";
        _this.razonSocial = "";
        _this.nombreArchivoSeleccionado = "";
        _this.fileKeySeleccionado = "";
        _this.descripcionSeleccionado = "";
        _this.archivoSeleccionado = null;
        _this.listaArchivos = [];
        _this.informe = new InformeComercial();
        _this.cartaPresentacion = new CartaPresentacion();
        _this.nuevoAtributoCampo = new NuevoProduccion();
        _this.nuevoAtributoAcompio = new NuevoAcopio();
        _this.searchTerm = new FormControl();
        _this.myLocalidades = [];
        _this.mensajeError = "";
        _this.tryDoctype = "";
        _this.keyword = "Nombre";
        _this.data = [];
        _this.localidades = [];
        _this.autocompleteNotFoundText = "No encontrado";
        _this.proveedorId = 0;
        _this.estadoSISA = "";
        _this.esCorredor = false;
        _this.esMultiFirma = false;
        _this.mensajeComponent = new MensajeComponent();
        return _this;
    }
    EmpresaGranosComponent.prototype.checkPermisos = function () {
        this.securityService.tienePermisoRedirect("ALTA EMPRESA GRANOS");
    };
    EmpresaGranosComponent.prototype.setTabs = function () {
        this.setMenuSeccionTab("alta-empresa", "Alta Empresa");
    };
    EmpresaGranosComponent.prototype.ngOnInit = function () {
        var _this = this;
        this.esCorredor = sessionStorage.getItem("tipoUsuario") === "CORR";
        this.esMultiFirma = this.securityService.tienePermiso("CONSULTAR VENDEDOR PENDIENTES");
        this.route.params.forEach(function (params) {
            if (params["id"] > 0)
                _this.proveedorId = params["id"];
        });
        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList([]);
        this.obtenerCampanias();
        this.obtenerArchivosSubidos();
        this.cargarSolicitudUsuario();
        this.obtenerInfoProveedor();
        this.addFieldValue();
        this.addFieldValueAlm();
        this.agregarCampoCartaPresentacion();
        this.agregarAcopioCartaPresentacion();
        //Agarramos los input que son de autocomplete de localidad (que usan un componente aparte) y les ponemos en off el autocomplete de chrome, para que no rellene formularios
        setTimeout(function () {
            var autocompletesLocalidad = document.querySelectorAll('[placeholder="Localidad"]');
            for (var i = 0; i < autocompletesLocalidad.length; i++) {
                autocompletesLocalidad[i].setAttribute("autocomplete", "chrome-off");
            }
        }, 1000);
    };
    Object.defineProperty(EmpresaGranosComponent.prototype, "email", {
        get: function () {
            return this.firstFormGroup.get("email");
        },
        enumerable: false,
        configurable: true
    });
    Object.defineProperty(EmpresaGranosComponent.prototype, "password", {
        get: function () {
            return this.secondFormGroup.get("password");
        },
        enumerable: false,
        configurable: true
    });
    EmpresaGranosComponent.prototype.selectEventProduccion = function (item, index) {
        this.informe.NuevosCampos[index].LocalidadId = item.LocalidadId;
    };
    EmpresaGranosComponent.prototype.selectEventAlmacenamiento = function (item, index) {
        this.informe.NuevosAcopios[index].LocalidadID = item.LocalidadId;
    };
    EmpresaGranosComponent.prototype.onChangeSearchProduccion = function (term) {
        var _this = this;
        if (term.length > 2) {
            this.unsubscribe();
            this.subscription = this.service.searchLocalidad(term).subscribe(function (result) {
                _this.data = result;
            }, function (error) {
                _this.spinnerSmallComponent.hideIt();
                _this.mensajeComponent.setErrorMsg(error.message);
            });
        }
    };
    EmpresaGranosComponent.prototype.onChangeSearchAlmacenamiento = function (term) {
        var _this = this;
        if (term.length > 2) {
            this.unsubscribe();
            this.subscription = this.service.searchLocalidad(term).subscribe(function (result) {
                _this.data = result;
            }, function (error) {
                _this.spinnerSmallComponent.hideIt();
                _this.mensajeComponent.setErrorMsg(error.message);
            });
        }
    };
    EmpresaGranosComponent.prototype.selectEventInforme = function (item) {
        this.informe.localidadId = item.LocalidadId;
    };
    EmpresaGranosComponent.prototype.onChangeSearchInforme = function (term) {
        var _this = this;
        if (term.length > 2) {
            this.unsubscribe();
            this.subscription = this.service.searchLocalidad(term).subscribe(function (result) {
                _this.data = result;
            }, function (error) {
                _this.spinnerSmallComponent.hideIt();
                _this.mensajeComponent.setErrorMsg(error.message);
            });
        }
    };
    EmpresaGranosComponent.prototype.handleFileInput = function (files, fileKey) {
        var _this = this;
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerSmallComponent.showIt();
        this.unsubscribe();
        this.subscription = this.service
            .postFile(files, fileKey, this.proveedorId)
            .subscribe(function (result) {
            _this.spinnerSmallComponent.hideIt();
            if (result.logout == true) {
                _this.sessionDataService.logout();
            }
            else if (result.error != undefined &&
                result.error != "") {
                _this.mensajeComponent.setErrorMsg(result.error);
            }
            else if (result.info != undefined) {
                _this.mensajeComponent.setInfoMsg(result.info);
            }
            else {
                _this.mensajeComponent.setMsgsEmpty();
                _this.mensajeComponent.setSuccessMsg(result.data);
                _this.obtenerArchivosSubidos();
            }
        }, function (error) {
            _this.spinnerSmallComponent.hideIt();
            _this.mensajeComponent.setErrorMsg(error.message);
        });
    };
    EmpresaGranosComponent.prototype.obtenerMateriales = function () {
        var _this = this;
        //Sacamos lo de la lista de campaña, ya que ahora son independientes
        this.subscription = this.service.obtenerMateriales().subscribe(function (result) {
            var obj = JSON.parse(result);
            // this.listaCampanias = new Array();
            obj.Datos.forEach(function (element) {
                var mat = new Material();
                mat.Id = element.MaterialId;
                mat.Descripcion = element.Descripcion;
                mat.CampaniaActual = element.CampaniaActual;
                mat.CampaniaIdActual = element.CampaniaIdActual;
                _this.listaMateriales.push(mat);
                /* let cam = {
                     CampaniaActual: element.CampaniaActual,
                     CampaniaIdActual: element.CampaniaIdActual,
                 };
                 this.listaCampanias.push(cam);*/
            });
            /*const listaCampanias2 = [];
            const map = new Map();
            for (const item of this.listaCampanias) {
                if (!map.has(item.CampaniaIdActual)) {
                    map.set(item.CampaniaIdActual, true); // set any value to Map
                    listaCampanias2.push({
                        CampaniaActual: item.CampaniaActual,
                        CampaniaIdActual: item.CampaniaIdActual,
                    });
                }
            }
            this.listaCampanias = listaCampanias2;*/
        }, function (error) {
            _this.mensajeComponent.setErrorMsg(error.message);
        });
    };
    EmpresaGranosComponent.prototype.obtenerCampanias = function () {
        var _this = this;
        this.subscription = this.service.obtenerCampanias().subscribe(function (result) {
            var obj = result;
            _this.listaCampanias = new Array();
            obj.forEach(function (element) {
                var cam = {
                    CampaniaActual: element.Descripcion,
                    CampaniaIdActual: element.CampaniaId,
                };
                _this.listaCampanias.push(cam);
            });
            _this.obtenerMateriales();
        }, function (error) {
            _this.mensajeComponent.setErrorMsg(error.message);
        });
    };
    EmpresaGranosComponent.prototype.obtenerInfoProveedor = function () {
        var _this = this;
        this.subscription = this.service
            .obtenerInfoProveedor(this.proveedorId)
            .subscribe(function (result) {
            _this.CBUSISA = result.ProveedorCBU;
            _this.estadoSISA = result.EstadoSISA;
            _this.proveedorCUIT = result.ProveedorCUIT;
            _this.razonSocial = result.RazonSocial;
        }, function (error) {
            _this.mensajeComponent.setErrorMsg(error.message);
        });
    };
    EmpresaGranosComponent.prototype.generarInformeComercial = function () {
        var _this = this;
        this.spinnerModal.showIt();
        this.unsubscribe();
        this.informe.NuevosAcopios.forEach(function (campo) {
            campo.CampaniaID = _this.campaniaActual;
        });
        this.informe.NuevosCampos.forEach(function (campo) {
            campo.CampaniaId = _this.campaniaActual;
        });
        this.informe.CampaniaId = this.campaniaActual;
        for (var _i = 0, _a = this.listaCampanias; _i < _a.length; _i++) {
            var item = _a[_i];
            if (item.CampaniaIdActual == this.campaniaActual) {
                this.informe.Campania = item.CampaniaActual;
            }
        }
        if (this.validarInforme()) {
            this.spinnerModal.hideIt();
            return;
        }
        this.mensajeError = "";
        this.subscription = this.service
            .generarInformeComercial(this.informe, this.proveedorId)
            .subscribe(function (result) {
            _this.spinnerModal.hideIt();
            if (result.error) {
                _this.mensajeError = result.error;
            }
            else {
                var byteArray = new Uint8Array(result.data);
                var blob = new Blob([byteArray], {
                    type: "application/pdf",
                });
                if (window.navigator.msSaveOrOpenBlob) {
                    // IE11
                    window.navigator.msSaveOrOpenBlob(blob, "Informe comercial" + ".pdf");
                }
                else {
                    var url = window.URL.createObjectURL(blob);
                    var link = document.createElement("a");
                    document.body.appendChild(link);
                    link.href = url;
                    link.download = "Informe comercial" + ".pdf";
                    link.click();
                    setTimeout(function () {
                        window.URL.revokeObjectURL(url);
                    }, 0);
                    return false;
                }
            }
        }, function (error) {
            _this.spinnerModal.hideIt();
            _this.mensajeError = error.message;
        });
    };
    EmpresaGranosComponent.prototype.descargarArchivo = function (archivo) {
        var _this = this;
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerSmallComponent.showIt();
        this.unsubscribe();
        var archivoID = archivo.Id;
        var fileKey = archivo.FileKey;
        this.subscription = this.service
            .descargarArchivoSubido(fileKey, archivoID)
            .subscribe(function (result) {
            _this.spinnerSmallComponent.hideIt();
            if (result.logout == true) {
                _this.sessionDataService.logout();
            }
            else if (result.error != undefined &&
                result.error != "") {
                _this.mensajeComponent.setErrorMsg(result.error);
            }
            else if (result.info != undefined) {
                _this.mensajeComponent.setInfoMsg(result.info);
            }
            else {
                var byteArray = new Uint8Array(result.FileContents);
                var blob = new Blob([byteArray], {
                    type: "application/octet-stream",
                });
                if (window.navigator.msSaveOrOpenBlob) {
                    // IE11
                    window.navigator.msSaveOrOpenBlob(blob, result.FileDownloadName);
                }
                else {
                    var url = window.URL.createObjectURL(blob);
                    var link = document.createElement("a");
                    document.body.appendChild(link);
                    link.href = url;
                    link.download = result.FileDownloadName;
                    link.click();
                    setTimeout(function () {
                        window.URL.revokeObjectURL(url);
                    }, 0);
                    return false;
                }
            }
        }, function (error) {
            _this.spinnerSmallComponent.hideIt();
            _this.mensajeComponent.setErrorMsg(error.message);
        });
    };
    EmpresaGranosComponent.prototype.eliminarArchivo = function (archivo) {
        this.fileKeySeleccionado = archivo.FileKey;
        this.nombreArchivoSeleccionado = archivo.Nombre;
        this.archivoSeleccionado = archivo;
        document.getElementById("openModalConfirmModal").click();
    };
    EmpresaGranosComponent.prototype.eliminarArchivoSeleccionado = function () {
        var _this = this;
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerSmallComponent.showIt();
        this.unsubscribe();
        var archivoID = this.archivoSeleccionado.Id;
        this.subscription = this.service
            .eliminarArchivoSubido(archivoID, this.proveedorId)
            .subscribe(function (result) {
            _this.spinnerSmallComponent.hideIt();
            if (result.logout == true) {
                _this.sessionDataService.logout();
            }
            else if (result.error != undefined &&
                result.error != "") {
                _this.mensajeComponent.setErrorMsg(result.error);
            }
            else if (result.info != undefined) {
                _this.mensajeComponent.setInfoMsg(result.info);
            }
            else {
                _this.obtenerArchivosSubidos();
                _this.fileKeySeleccionado = "";
                _this.nombreArchivoSeleccionado = "";
                _this.descripcionSeleccionado = "";
                document
                    .getElementById("openModalConfirmModal")
                    .click();
            }
        }, function (error) {
            _this.spinnerSmallComponent.hideIt();
            _this.mensajeComponent.setErrorMsg(error.message);
        });
    };
    EmpresaGranosComponent.prototype.obtenerArchivosSubidos = function () {
        var _this = this;
        this.subscription = this.service
            .obtenerArchivosSubidos("", this.proveedorId)
            .subscribe(function (result) {
            _this.listaArchivos = new Array();
            result.forEach(function (element) {
                var archivo = new Archivo();
                archivo = element;
                _this.listaArchivos.push(archivo);
            });
        }, function (error) {
            _this.mensajeComponent.setErrorMsg(error.message);
        });
    };
    EmpresaGranosComponent.prototype.redirigirAEstado = function () {
        if (this.esMultiFirma) {
            this.navService.navegarSeccion("/dato-fiscal/vendedores-pendientes");
        }
        else {
            this.navService.navegarSeccion("/estado-solicitud");
        }
    };
    EmpresaGranosComponent.prototype.buscarArchivoPorFileKey = function (fileKey) {
        return this.listaArchivos.find(function (x) { return x.FileKey == fileKey; }).Nombre;
    };
    EmpresaGranosComponent.prototype.onSubmit = function () {
        var _this = this;
        if (this.validarTyC()) {
            this.spinnerModal.hideIt();
            return;
        }
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerSmallComponent.showIt();
        this.unsubscribe();
        var datos = {
            Empleados: this.empleados,
            Funcionarios: this.funcionarios,
            VinculoConEmpleadosDeMolinos: this.relacionConEmpleadosChecked,
            VinculoConFuncionariosPublicos: this.relacionConFuncionariosChecked,
        };
        this.subscription = this.service
            .enviarSolicitud(datos, this.proveedorId)
            .subscribe(function (result) {
            _this.spinnerSmallComponent.hideIt();
            if (result.logout == true) {
                _this.sessionDataService.logout();
            }
            else if (result.error != undefined &&
                result.error != "") {
                _this.mensajeComponent.setErrorMsg(result.error);
            }
            else if (result.info != undefined) {
                _this.mensajeComponent.setInfoMsg(result.info);
            }
            else {
                _this.mensajeComponent.setMsgsEmpty();
                document
                    .getElementById("openModalNotificacion")
                    .click();
            }
        }, function (error) {
            _this.spinnerSmallComponent.hideIt();
            _this.mensajeComponent.setErrorMsg(error.message);
        });
    };
    EmpresaGranosComponent.prototype.addFieldValue = function () {
        this.informe.NuevosCampos.push(this.newAttribute);
        this.newAttribute = new NuevoProduccion();
    };
    EmpresaGranosComponent.prototype.deleteFieldValue = function (index) {
        this.informe.NuevosCampos.splice(index, 1);
    };
    EmpresaGranosComponent.prototype.addFieldValueAlm = function () {
        this.informe.NuevosAcopios.push(this.newAttributeAlm);
        this.newAttributeAlm = new NuevoAcopio();
    };
    EmpresaGranosComponent.prototype.deleteFieldValueAlm = function (index) {
        this.informe.NuevosAcopios.splice(index, 1);
    };
    EmpresaGranosComponent.prototype.validarInforme = function () {
        if (this.informe.direccion == "" || !this.informe.direccion) {
            this.mensajeError = "No completo la direccion.";
            return true;
        }
        if (!this.informe.codigoPostal || this.informe.codigoPostal == "") {
            this.mensajeError = "No completo el codigo postal.";
            return true;
        }
        if (!this.informe.localidadId ||
            this.informe.localidadId == null ||
            this.informe.localidadId == 0) {
            this.mensajeError =
                "No completo la Localidad en Domicilio Actividad.";
            return true;
        }
        if (!this.informe.ContactoComercial.Apellido ||
            this.informe.ContactoComercial.Apellido == "") {
            this.mensajeError = "No completo el Apellido del contacto.";
            return true;
        }
        if (!this.informe.ContactoComercial.Nombres ||
            this.informe.ContactoComercial.Nombres == "") {
            this.mensajeError = "No completo el Nombre del contacto.";
            return true;
        }
        if (!this.informe.ContactoComercial.Puesto ||
            this.informe.ContactoComercial.Puesto == "") {
            this.mensajeError = "No completo el Puesto del contacto.";
            return true;
        }
        if (!this.informe.ContactoComercial.Telefono1 ||
            this.informe.ContactoComercial.Telefono1 == "") {
            this.mensajeError = "No completo el Telefono del contacto.";
            return true;
        }
        if (this.informe.CampaniaId == 0 || !this.informe.direccion) {
            this.mensajeError = "No completo la Campaña Actual.";
            return true;
        }
        var filaError = 0;
        for (var _i = 0, _a = this.informe.NuevosCampos; _i < _a.length; _i++) {
            var item = _a[_i];
            filaError++;
            if (item.MaterialId == null || item.MaterialId == 0) {
                this.mensajeError = "Debe completar el grano en la fila " + filaError + " de capacidad productiva.";
                return true;
            }
            if (item.LocalidadId == null || item.LocalidadId == 0) {
                this.mensajeError = "Debe completar la localidad en la fila " + filaError + " de capacidad productiva.";
                return true;
            }
            if (item.Toneladas == null || item.Toneladas == 0) {
                this.mensajeError = "Debe completar las toneladas en la fila " + filaError + " de capacidad productiva.";
                return true;
            }
            if (item.ArrendaPropia == null) {
                this.mensajeError = "Debe completar la condicion en la fila " + filaError + " de capacidad productiva.";
                return true;
            }
        }
        filaError = 0;
        this.informe.NuevosAcopios = this.informe.NuevosAcopios.filter(function (item) { return ((item.LocalidadID == null || item.LocalidadID == 0) && (item.Toneladas == null || item.Toneladas == 0)) == false; });
        //
        for (var _b = 0, _c = this.informe.NuevosAcopios; _b < _c.length; _b++) {
            var item = _c[_b];
            filaError++;
            if (item.LocalidadID == null || item.LocalidadID == 0) {
                this.mensajeError = "Debe completar la localidad en la fila " + filaError + " de capacidad planta.";
                return true;
            }
            if (item.Toneladas == null || item.Toneladas == 0) {
                this.mensajeError = "Debe completar las Toneladas en la fila " + filaError + "  de capacidad planta.";
                return true;
            }
            if (item.ArrendaPropia == null) {
                this.mensajeError = "Debe completar la condicion en la fila " + filaError + " de capacidad planta.";
                return true;
            }
        }
        return false;
    };
    EmpresaGranosComponent.prototype.addFieldValueEmpleados = function () {
        this.empleados.push(this.newAttributeEmpleados);
        this.newAttributeEmpleados = new RelacionConEmpleados();
    };
    EmpresaGranosComponent.prototype.deleteFieldValueEmpleados = function (index) {
        this.empleados.splice(index, 1);
    };
    EmpresaGranosComponent.prototype.isVisibleTablaEmpleados = function () {
        return this.relacionConEmpleadosChecked;
    };
    EmpresaGranosComponent.prototype.relacionConEmpleadosCheckSi = function (event) {
        this.relacionConEmpleadosChecked = true;
        if (this.empleados.length == 0) {
            this.addFieldValueEmpleados();
        }
    };
    EmpresaGranosComponent.prototype.relacionConEmpleadosCheckNo = function (event) {
        this.relacionConEmpleadosChecked = false;
    };
    EmpresaGranosComponent.prototype.addFieldValueFuncionarios = function () {
        this.funcionarios.push(this.newAttributeFuncionarios);
        this.newAttributeFuncionarios = new RelacionConFuncionarios();
    };
    EmpresaGranosComponent.prototype.deleteFieldValueFuncionarios = function (index) {
        this.funcionarios.splice(index, 1);
    };
    EmpresaGranosComponent.prototype.isVisibleTablaFuncionarios = function () {
        return this.relacionConFuncionariosChecked;
    };
    EmpresaGranosComponent.prototype.relacionConFuncionariosCheckSi = function (event) {
        this.relacionConFuncionariosChecked = true;
        if (this.funcionarios.length == 0) {
            this.addFieldValueFuncionarios();
        }
    };
    EmpresaGranosComponent.prototype.relacionConFuncionariosCheckNo = function (event) {
        this.relacionConFuncionariosChecked = false;
    };
    EmpresaGranosComponent.prototype.validarTyC = function () {
        if (this.relacionConEmpleadosChecked == null) {
            this.mensajeComponent.setErrorMsg("Debe completar Vínculos a declarar con Empleados de Molinos agro S.A.");
            return true;
        }
        if (this.relacionConEmpleadosChecked == false) {
            this.empleados = [];
        }
        if (this.relacionConEmpleadosChecked == true) {
            if (this.empleados.length == 0) {
                this.mensajeComponent.setErrorMsg("Debe completar Vínculos a declarar con Empleados de Molinos agro S.A.");
            }
            else {
                for (var _i = 0, _a = this.empleados; _i < _a.length; _i++) {
                    var item = _a[_i];
                    if (item.NombreProveedora == null ||
                        item.NombreProveedora == "") {
                        this.mensajeComponent.setErrorMsg("Debe completar el nombre en todos los items de Vínculos a declarar con Empleados de Molinos agro.");
                        return true;
                    }
                    if (item.CargoProveedora == null ||
                        item.CargoProveedora == "") {
                        this.mensajeComponent.setErrorMsg("Debe completar el cargo en todos los items de Vínculos a declarar con Empleados de Molinos agro.");
                        return true;
                    }
                    if (item.NombreMolinos == null ||
                        item.NombreMolinos == "") {
                        this.mensajeComponent.setErrorMsg("Debe completar el nombre en todos los items de Vínculos a declarar con Empleados de Molinos agro.");
                        return true;
                    }
                    if (item.Vinculo == null || item.Vinculo == "") {
                        this.mensajeComponent.setErrorMsg("Debe completar el vinculo en todos los items de Vínculos a declarar con Empleados de Molinos agro.");
                        return true;
                    }
                }
            }
        }
        if (this.relacionConFuncionariosChecked == null) {
            this.mensajeComponent.setErrorMsg("Debe completar Vínculos a declarar con Funcionarios Públicos");
            return true;
        }
        if (this.relacionConFuncionariosChecked == false) {
            this.funcionarios = [];
        }
        if (this.relacionConFuncionariosChecked == true) {
            if (this.funcionarios.length == 0) {
                this.mensajeComponent.setErrorMsg("Debe completar Vínculos a declarar con Funcionarios Públicos");
            }
            else {
                for (var _b = 0, _c = this.funcionarios; _b < _c.length; _b++) {
                    var item = _c[_b];
                    if (item.NombreFirma == null || item.NombreFirma == "") {
                        this.mensajeComponent.setErrorMsg("Debe completar el nombre en todos los items de Vínculos a declarar con Funcionarios Públicos.");
                        return true;
                    }
                    if (item.CargoFirma == null || item.CargoFirma == "") {
                        this.mensajeComponent.setErrorMsg("Debe completar el cargo en todos los items de Vínculos a declarar con Funcionarios Públicos.");
                        return true;
                    }
                    if (item.NombreFuncionario == null ||
                        item.NombreFuncionario == "") {
                        this.mensajeComponent.setErrorMsg("Debe completar el nombre en todos los items de Vínculos a declarar con Funcionarios Públicos.");
                        return true;
                    }
                    if (item.CargoFuncionario == null ||
                        item.CargoFuncionario == "") {
                        this.mensajeComponent.setErrorMsg("Debe completar el cargo en todos los items de Vínculos a declarar con Funcionarios Públicos.");
                        return true;
                    }
                    if (item.Vinculo == null || item.Vinculo == "") {
                        this.mensajeComponent.setErrorMsg("Debe completar el vinculo en todos los items de Vínculos a declarar con Funcionarios Públicos.");
                        return true;
                    }
                }
            }
        }
        if (this.codigoDeConducta == false) {
            this.mensajeComponent.setErrorMsg("Debe aceptar el Código de Conducta de Proveedores de Molinos agro S.A.");
            return true;
        }
        return false;
    };
    EmpresaGranosComponent.prototype.abrirCodigoConducta = function (siempre) {
        if (this.codigoConductaVisto == false || siempre) {
            this.codigoConductaVisto = true;
            var filePath = location.origin +
                "/Documentacion/CodigoDeConductaParaProveedoresMOA.pdf";
            console.log(filePath);
            var url = location.origin +
                "/officetohtml/popup.html?filePath=" +
                filePath;
            this.tryDoctype = url;
            document.getElementById("iframeConducta").setAttribute("src", url);
            document.getElementById("openModalconductaModal").click();
        }
    };
    EmpresaGranosComponent.prototype.cargarSolicitudUsuario = function () {
        var _this = this;
        this.subscription = this.service.cargarSolicitudUsuario("", this.proveedorId).subscribe(function (result) {
            if (result.VinculoConEmpleadosDeMolinos != null) {
                _this.codigoConductaVisto = true;
                _this.codigoDeConducta = true;
                _this.relacionConEmpleadosChecked =
                    result.VinculoConEmpleadosDeMolinos;
                if (result.VinculoConEmpleadosDeMolinos) {
                    document
                        .getElementById("radioEmpleadosSi")
                        .setAttribute("checked", "true");
                }
                else {
                    document
                        .getElementById("radioEmpleadosNo")
                        .setAttribute("checked", "true");
                }
                _this.relacionConFuncionariosChecked =
                    result.VinculoConFuncionariosPublicos;
                if (result.VinculoConFuncionariosPublicos) {
                    document
                        .getElementById("radioFuncionariosSi")
                        .setAttribute("checked", "true");
                }
                else {
                    document
                        .getElementById("radioFuncionariosNo")
                        .setAttribute("checked", "true");
                }
                _this.empleados = result.Empleados;
                _this.funcionarios = result.Funcionarios;
            }
        }, function (error) {
            _this.mensajeComponent.setErrorMsg(error.message);
        });
    };
    EmpresaGranosComponent.prototype.agregarCampoCartaPresentacion = function () {
        this.cartaPresentacion.nuevosCampos.push(this.nuevoAtributoCampo);
        this.nuevoAtributoCampo = new NuevoProduccion();
    };
    EmpresaGranosComponent.prototype.borrarCampoCartaPresentacion = function (index) {
        this.cartaPresentacion.nuevosCampos.splice(index, 1);
    };
    EmpresaGranosComponent.prototype.agregarAcopioCartaPresentacion = function () {
        this.cartaPresentacion.nuevosAcopios.push(this.nuevoAtributoAcompio);
        this.nuevoAtributoAcompio = new NuevoAcopio();
    };
    EmpresaGranosComponent.prototype.borrarAcopioCartaPresentacion = function (index) {
        this.cartaPresentacion.nuevosAcopios.splice(index, 1);
    };
    EmpresaGranosComponent.prototype.selectEventCampoCartaPresentacion = function (item, index) {
        this.cartaPresentacion.nuevosCampos[index].LocalidadId =
            item.LocalidadId;
    };
    EmpresaGranosComponent.prototype.selectEventAcopioCartaPresentacion = function (item, index) {
        this.cartaPresentacion.nuevosAcopios[index].LocalidadID =
            item.LocalidadId;
    };
    EmpresaGranosComponent.prototype.onChangeLocalidad = function (term) {
        var _this = this;
        if (term.length > 2) {
            this.unsubscribe();
            this.subscription = this.service.searchLocalidad(term).subscribe(function (result) {
                _this.localidades = result;
            }, function (error) {
                _this.spinnerSmallComponent.hideIt();
                _this.mensajeComponent.setErrorMsg(error.message);
            });
        }
    };
    EmpresaGranosComponent.prototype.generarCartaPresentacion = function () {
        var _this = this;
        this.spinnerCartaPresentacion.showIt();
        this.unsubscribe();
        this.cartaPresentacion.nuevosAcopios.forEach(function (campo) {
            campo.CampaniaID = _this.campaniaActual;
        });
        this.cartaPresentacion.nuevosCampos.forEach(function (campo) {
            campo.CampaniaId = _this.campaniaActual;
        });
        this.cartaPresentacion.campaniaID = this.campaniaActual;
        if (this.validarCartaPresentacion()) {
            this.spinnerCartaPresentacion.hideIt();
            return;
        }
        this.mensajeError = "";
        this.subscription = this.service
            .generarCartaPresentacion(this.cartaPresentacion, this.proveedorId)
            .subscribe(function (result) {
            _this.spinnerCartaPresentacion.hideIt();
            if (result.error) {
                _this.mensajeError = result.error;
            }
            else {
                var byteArray = new Uint8Array(result.data);
                var blob = new Blob([byteArray], {
                    type: "application/pdf",
                });
                if (window.navigator.msSaveOrOpenBlob) {
                    // IE11
                    window.navigator.msSaveOrOpenBlob(blob, "Carta presentación.pdf");
                }
                else {
                    var url = window.URL.createObjectURL(blob);
                    var link = document.createElement("a");
                    document.body.appendChild(link);
                    link.href = url;
                    link.download = "Carta presentación.pdf";
                    link.click();
                    setTimeout(function () {
                        window.URL.revokeObjectURL(url);
                    }, 0);
                    return false;
                }
            }
        }, function (error) {
            _this.spinnerCartaPresentacion.hideIt();
            _this.mensajeError = error.message;
        });
    };
    EmpresaGranosComponent.prototype.validarCartaPresentacion = function () {
        //Corredor
        if (this.cartaPresentacion.corredorBolsa == "" ||
            !this.cartaPresentacion.corredorBolsa) {
            this.mensajeError = "No completo el campo bolsa.";
            return true;
        }
        if (this.cartaPresentacion.corredorNroRegistro == "" ||
            !this.cartaPresentacion.corredorNroRegistro) {
            this.mensajeError = "No completo el número de registro.";
            return true;
        }
        //Vendedor
        if (this.cartaPresentacion.vendedorActividad == "" ||
            !this.cartaPresentacion.vendedorActividad) {
            this.mensajeError = "No seleccionó la actividad.";
            return true;
        }
        if (this.cartaPresentacion.vendedorDomicilioFiscal == "" ||
            !this.cartaPresentacion.vendedorDomicilioFiscal) {
            this.mensajeError = "No completo el domicilio fiscal.";
            return true;
        }
        if (this.cartaPresentacion.vendedorMailContacto == "" ||
            !this.cartaPresentacion.vendedorMailContacto) {
            this.mensajeError = "No completo el mail de contacto.";
            return true;
        }
        if (this.cartaPresentacion.vendedorTelefonoContacto == "" ||
            !this.cartaPresentacion.vendedorTelefonoContacto) {
            this.mensajeError = "No completo el teléfono de contacto.";
            return true;
        }
        if (this.cartaPresentacion.campaniaID == 0 ||
            !this.cartaPresentacion.campaniaID) {
            this.mensajeError = "No completo la Campaña Actual.";
            return true;
        }
        var filaError = 0;
        for (var _i = 0, _a = this.cartaPresentacion.nuevosCampos; _i < _a.length; _i++) {
            var item = _a[_i];
            filaError++;
            if (item.MaterialId == null || item.MaterialId == 0) {
                this.mensajeError = "Debe completar el grano en la fila " + filaError + " de capacidad productiva.";
                return true;
            }
            if (item.LocalidadId == null || item.LocalidadId == 0) {
                this.mensajeError = "Debe completar la localidad en la fila " + filaError + " de capacidad productiva.";
                return true;
            }
            if (item.Toneladas == null || item.Toneladas == 0) {
                this.mensajeError = "Debe completar las toneladas en la fila " + filaError + " de capacidad productiva.";
                return true;
            }
            if (item.ArrendaPropia == null) {
                this.mensajeError = "Debe completar la condicion en la fila " + filaError + " de capacidad productiva.";
                return true;
            }
        }
        filaError = 0;
        this.cartaPresentacion.nuevosAcopios = this.cartaPresentacion.nuevosAcopios.filter(function (item) { return ((item.LocalidadID == null || item.LocalidadID == 0) && (item.Toneladas == null || item.Toneladas == 0)) == false; });
        for (var _b = 0, _c = this.cartaPresentacion.nuevosAcopios; _b < _c.length; _b++) {
            var item = _c[_b];
            if (item.LocalidadID == null || item.LocalidadID == 0) {
                this.mensajeError = "Debe completar la localidad en la fila " + filaError + " de capacidad planta.";
                return true;
            }
            if (item.Toneladas == null || item.Toneladas == 0) {
                this.mensajeError = "Debe completar las Toneladas en la fila " + filaError + "  de capacidad planta.";
                return true;
            }
            if (item.ArrendaPropia == null) {
                this.mensajeError = "Debe completar la condicion en la fila " + filaError + " de capacidad planta.";
                return true;
            }
        }
        return false;
    };
    __decorate([
        ViewChild("msjEmpresaGranos"),
        __metadata("design:type", MensajeComponent)
    ], EmpresaGranosComponent.prototype, "mensajeComponent", void 0);
    __decorate([
        ViewChild(SpinnerSmallComponent),
        __metadata("design:type", SpinnerSmallComponent)
    ], EmpresaGranosComponent.prototype, "spinnerSmallComponent", void 0);
    __decorate([
        ViewChild("spinnerModal"),
        __metadata("design:type", SpinnerSmallComponent)
    ], EmpresaGranosComponent.prototype, "spinnerModal", void 0);
    __decorate([
        ViewChild("spinnerCartaPresentacion"),
        __metadata("design:type", SpinnerSmallComponent)
    ], EmpresaGranosComponent.prototype, "spinnerCartaPresentacion", void 0);
    EmpresaGranosComponent = __decorate([
        Component({
            selector: "app-empresa-granos",
            templateUrl: "empresa-granos.component.html",
            styleUrls: [
                "empresa-granos.component.css",
                "../../../../Content/css/bootstrap.min.css",
            ],
            providers: [EmpresaGranosService],
        }),
        __metadata("design:paramtypes", [EmpresaGranosService,
            NavService,
            ActivatedRoute,
            SessionDataService,
            SecurityService,
            FloatMsgService,
            ModalService])
    ], EmpresaGranosComponent);
    return EmpresaGranosComponent;
}(ListBaseComponent));
export { EmpresaGranosComponent };
//# sourceMappingURL=empresa-granos.component.js.map