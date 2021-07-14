import { Component, ViewChild } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";
import { Router, ActivatedRoute, Params } from "@angular/router";
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
import { EmpresaNoGranosService } from "./empresa-no-granos.service";
import { ContactoComercial } from "../../common/models/contactoComercial";
import { RelacionConEmpleados } from "../../common/models//RelacionConEmpleados";
import { RelacionConFuncionarios } from "../../common/models/relacionConFuncionarios";
import { forEach } from "@angular/router/src/utils/collection";
import { CartaPresentacion } from "../../common/models/cartaPresentacion";
import * as $ from 'jquery';

@Component({
    selector: "app-empresa-no-granos",
    templateUrl: "empresa-no-granos.component.html",
    styleUrls: [
        "empresa-no-granos.component.css",
        "../../../../Content/css/bootstrap.min.css",
    ],
    providers: [EmpresaNoGranosService],
})
export class EmpresaNoGranosComponent extends ListBaseComponent {
    firstFormGroup: FormGroup;
    secondFormGroup: FormGroup;
    http: any;
    fileToUpload: File;
    listaMateriales: Array<Material> = [];
    listaCampanias: any = [];
    campaniaActual: number;
    empleados: Array<RelacionConEmpleados> = [];
    funcionarios: Array<RelacionConFuncionarios> = [];
    // private acopiosArray: Array<NuevoAcopio> = [];
    private newAttributeEmpleados: RelacionConEmpleados = new RelacionConEmpleados();
    private newAttributeFuncionarios: RelacionConFuncionarios = new RelacionConFuncionarios();
    private newAttributeAlm: NuevoAcopio = new NuevoAcopio();
    private newAttribute: NuevoProduccion = new NuevoProduccion();
    relacionConEmpleadosChecked: boolean = null;
    relacionConFuncionariosChecked: boolean = null;
    codigoConductaVisto: boolean = false;
    codigoDeConducta: boolean = false;
    materialesData: any = null;
    CBUSISA: string = "";
    proveedorCUIT: string = "";
    razonSocial: string = "";

    nombreArchivoSeleccionado: string = "";
    fileKeySeleccionado: string = "";
    descripcionSeleccionado: string = "";
    archivoSeleccionado: Archivo = null;

    listaArchivos: Array<Archivo> = [];


    searchTerm: FormControl = new FormControl();
    myLocalidades = <any>[];
    mensajeError: string = "";
    tryDoctype: string = "";
    keyword = "Nombre";
    data = [];
    localidades: any = [];
    autocompleteNotFoundText = "No encontrado";
    private selectUndefinedOptionValue: any;
    proveedorId: number = 0;
    estadoSISA: string = "";
    IdSituacionIVA: number = 0;
    IdIngresoBruto: number = 0;
    CBUNoGranos: string = "";
    esUsuarioCompras: boolean = false;
    ingresoAPlanta: boolean = false;
    siperObligatorio: boolean = false; 

    esGuardarYNotificar: boolean = false;
    Comentarios: string = "";

    empresaSeleccionada: any;

    constructor(
        protected service: EmpresaNoGranosService,
        protected navService: NavService,
        private route: ActivatedRoute,
        protected sessionDataService: SessionDataService,
        protected securityService: SecurityService,
        protected floatMsgService: FloatMsgService,
        protected modalService: ModalService
    ) {
        super(
            service,
            navService,
            sessionDataService,
            securityService,
            floatMsgService,
            modalService
        );
        this.mensajeComponent = new MensajeComponent();
    }

    @ViewChild("msjEmpresaGranos")
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerSmallComponent)
    public spinnerSmallComponent: SpinnerSmallComponent;

    @ViewChild("spinnerModal")
    protected spinnerModal: SpinnerSmallComponent;

    @ViewChild("spinnerCartaPresentacion")
    protected spinnerCartaPresentacion: SpinnerSmallComponent;

    checkPermisos() {
        this.securityService.tienePermisoRedirect("ALTA EMPRESA NO GRANOS");
    }

    setTabs() {
        this.setMenuSeccionTab("alta-empresa", "Alta Empresa");
    }

    ngOnInit() {

        this.route.params.forEach((params: Params) => {
            if (params["id"] > 0) {
                this.proveedorId = params["id"];
            }
        });

        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList([]);

        this.obtenerArchivosSubidos();
        this.cargarSolicitudUsuario();
        this.obtenerInfoProveedor();


        //Si no tiene firmado el codigo de conducta, significa que necesitamos que lo haga el proveedor. Por lo tanto, solo permitimos que guarde los archivos y datos de IVA, IIBB y Cbu
        if (this.securityService.tienePermiso('ABM EMPRESAS')) {
            this.esUsuarioCompras = true;
        }


        //Agarramos los input que son de autocomplete de localidad (que usan un componente aparte) y les ponemos en off el autocomplete de chrome, para que no rellene formularios
        setTimeout(() => {
            var autocompletesLocalidad = document.querySelectorAll('[placeholder="Localidad"]');
            for (let i = 0; i < autocompletesLocalidad.length; i++) {
                autocompletesLocalidad[i].setAttribute("autocomplete", "chrome-off");
            }
        }, 1000);

    }

    get email() {
        return this.firstFormGroup.get("email");
    }
    get password() {
        return this.secondFormGroup.get("password");
    }

    handleFileInput(files: FileList, fileKey: string) {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerSmallComponent.showIt();

        this.unsubscribe();
        this.subscription = this.service
            .postFile(files, fileKey, this.proveedorId)
            .subscribe(
                (result) => {
                    this.spinnerSmallComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (
                        result.error != undefined &&
                        result.error != ""
                    ) {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.mensajeComponent.setMsgsEmpty();
                        this.mensajeComponent.setSuccessMsg(result.data);
                        this.obtenerArchivosSubidos();
                    }
                },
                (error) => {
                    this.spinnerSmallComponent.hideIt();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
    }

    obtenerInfoProveedor() {
        this.subscription = this.service
            .obtenerInfoProveedor(this.proveedorId)
            .subscribe(
                (result) => {
                    //this.CBUSISA = result.ProveedorCBU;
                    //this.estadoSISA = result.EstadoSISA;
                    this.proveedorCUIT = result.ProveedorCUIT;
                    this.razonSocial = result.RazonSocial;
                    this.ingresoAPlanta = result.IngresoAPlanta;
                    this.siperObligatorio = result.SiperObligatorio;
                },
                (error) => {
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
    }

    descargarArchivo(archivo: Archivo) {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerSmallComponent.showIt();
        this.unsubscribe();

        let archivoID: number = archivo.Id;
        let fileKey: string = archivo.FileKey;

        this.subscription = this.service
            .descargarArchivoSubido(fileKey, archivoID)
            .subscribe(
                (result) => {
                    this.spinnerSmallComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (
                        result.error != undefined &&
                        result.error != ""
                    ) {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        var byteArray = new Uint8Array(result.FileContents);
                        var blob = new Blob([byteArray], {
                            type: "application/octet-stream",
                        });

                        if (window.navigator.msSaveOrOpenBlob) {
                            // IE11
                            window.navigator.msSaveOrOpenBlob(
                                blob,
                                result.FileDownloadName
                            );
                        } else {
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
                },
                (error) => {
                    this.spinnerSmallComponent.hideIt();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
    }

    eliminarArchivo(archivo: Archivo) {
        this.fileKeySeleccionado = archivo.FileKey;
        this.nombreArchivoSeleccionado = archivo.Nombre;
        this.archivoSeleccionado = archivo;

        document.getElementById("openModalConfirmModal").click();
    }

    eliminarArchivoSeleccionado() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerSmallComponent.showIt();
        this.unsubscribe();


        let archivoID: number = this.archivoSeleccionado.Id;

        this.subscription = this.service
            .eliminarArchivoSubido(archivoID, this.proveedorId)
            .subscribe(
                (result) => {
                    this.spinnerSmallComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (
                        result.error != undefined &&
                        result.error != ""
                    ) {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.obtenerArchivosSubidos();
                        this.fileKeySeleccionado = "";
                        this.nombreArchivoSeleccionado = "";
                        this.descripcionSeleccionado = "";
                        document
                            .getElementById("openModalConfirmModal")
                            .click();
                    }
                },
                (error) => {
                    this.spinnerSmallComponent.hideIt();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
    }

    obtenerArchivosSubidos() {
        this.subscription = this.service
            .obtenerArchivosSubidos("", this.proveedorId)
            .subscribe(
                (result) => {
                    this.listaArchivos = new Array();

                    result.forEach((element) => {
                        let archivo = new Archivo();
                        archivo = element;
                        this.listaArchivos.push(archivo);
                    });
                },
                (error) => {
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
    }

    buscarArchivoPorFileKey(fileKey: string) {
        return this.listaArchivos.find((x) => x.FileKey == fileKey).Nombre;
    }

    onSubmit() {
        if (this.validar()) {
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
            CBU: this.CBUNoGranos,
            IdSituacionIVA: this.IdSituacionIVA,
            IdIngresoBruto: this.IdIngresoBruto,
            Comentarios: this.Comentarios
        };

        this.subscription = this.service
            .enviarSolicitud(datos, this.esGuardarYNotificar, this.proveedorId)
            .subscribe(
                (result) => {
                    this.spinnerSmallComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (
                        result.error != undefined &&
                        result.error != ""
                    ) {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.mensajeComponent.setMsgsEmpty();
                        document
                            .getElementById("openModalNotificacion")
                            .click();
                    }
                },
                (error) => {
                    this.spinnerSmallComponent.hideIt();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
    }


    addFieldValueEmpleados() {
        this.empleados.push(this.newAttributeEmpleados);
        this.newAttributeEmpleados = new RelacionConEmpleados();
    }

    deleteFieldValueEmpleados(index) {
        this.empleados.splice(index, 1);
    }

    isVisibleTablaEmpleados(): boolean {
        return this.relacionConEmpleadosChecked;
    }
    relacionConEmpleadosCheckSi(event) {
        this.relacionConEmpleadosChecked = true;
        if (this.empleados.length == 0) {
            this.addFieldValueEmpleados();
        }
    }
    relacionConEmpleadosCheckNo(event) {
        this.relacionConEmpleadosChecked = false;
    }

    addFieldValueFuncionarios() {
        this.funcionarios.push(this.newAttributeFuncionarios);
        this.newAttributeFuncionarios = new RelacionConFuncionarios();
    }

    deleteFieldValueFuncionarios(index) {
        this.funcionarios.splice(index, 1);
    }

    isVisibleTablaFuncionarios(): boolean {
        return this.relacionConFuncionariosChecked;
    }
    relacionConFuncionariosCheckSi(event) {
        this.relacionConFuncionariosChecked = true;
        if (this.funcionarios.length == 0) {
            this.addFieldValueFuncionarios();
        }
    }
    relacionConFuncionariosCheckNo(event) {
        this.relacionConFuncionariosChecked = false;
    }

    validar() {

        if (this.IdSituacionIVA == null || this.IdSituacionIVA <= 0) {
            this.mensajeComponent.setErrorMsg("Debe completar la Situacion de IVA");
            return true;
        }

        if (this.IdIngresoBruto == null || this.IdIngresoBruto <= 0) {
            this.mensajeComponent.setErrorMsg("Debe completar la Ingresos Brutos");
            return true;
        }

        if (this.CBUNoGranos == null || this.CBUNoGranos.length != 22) {
            this.mensajeComponent.setErrorMsg("Debe completar el CBU");
            return true;
        }
    
        if(!this.esGuardarYNotificar)
        {
            if (this.relacionConEmpleadosChecked == null) {
                this.mensajeComponent.setErrorMsg(
                    "Debe completar V\u00EDnculos a declarar con Empleados de Molinos agro S.A."
                );
                return true;
            }
            if (this.relacionConEmpleadosChecked == false) {
                this.empleados = [];
            }
            if (this.relacionConEmpleadosChecked == true) {
                if (this.empleados.length == 0) {
                    this.mensajeComponent.setErrorMsg(
                        "Debe completar V\u00EDnculos a declarar con Empleados de Molinos agro S.A."
                    );
                } else {
                    for (const item of this.empleados) {
                        if (
                            item.NombreProveedora == null ||
                            item.NombreProveedora == ""
                        ) {
                            this.mensajeComponent.setErrorMsg(
                                "Debe completar el nombre en todos los items de V\u00EDnculos a declarar con Empleados de Molinos agro."
                            );
                            return true;
                        }
                        if (
                            item.CargoProveedora == null ||
                            item.CargoProveedora == ""
                        ) {
                            this.mensajeComponent.setErrorMsg(
                                "Debe completar el cargo en todos los items de V\u00EDnculos a declarar con Empleados de Molinos agro."
                            );
                            return true;
                        }
                        if (
                            item.NombreMolinos == null ||
                            item.NombreMolinos == ""
                        ) {
                            this.mensajeComponent.setErrorMsg(
                                "Debe completar el nombre en todos los items de V\u00EDnculos a declarar con Empleados de Molinos agro."
                            );
                            return true;
                        }
                        if (item.Vinculo == null || item.Vinculo == "") {
                            this.mensajeComponent.setErrorMsg(
                                "Debe completar el vinculo en todos los items de V\u00EDnculos a declarar con Empleados de Molinos agro."
                            );
                            return true;
                        }
                    }
                }
            }

            if (this.relacionConFuncionariosChecked == null) {
                this.mensajeComponent.setErrorMsg(
                    "Debe completar V\u00EDnculos a declarar con Funcionarios P\u00FAblicos"
                );
                return true;
            }
            if (this.relacionConFuncionariosChecked == false) {
                this.funcionarios = [];
            }
            if (this.relacionConFuncionariosChecked == true) {
                if (this.funcionarios.length == 0) {
                    this.mensajeComponent.setErrorMsg(
                        "Debe completar V\u00EDnculos a declarar con Funcionarios P\u00FAblicos"
                    );
                } else {
                    for (const item of this.funcionarios) {
                        if (item.NombreFirma == null || item.NombreFirma == "") {
                            this.mensajeComponent.setErrorMsg(
                                "Debe completar el nombre en todos los items de V\u00EDnculos a declarar con Funcionarios P\u00FAblicos."
                            );
                            return true;
                        }
                        if (item.CargoFirma == null || item.CargoFirma == "") {
                            this.mensajeComponent.setErrorMsg(
                                "Debe completar el cargo en todos los items de V\u00EDnculos a declarar con Funcionarios P\u00FAblicos."
                            );
                            return true;
                        }
                        if (
                            item.NombreFuncionario == null ||
                            item.NombreFuncionario == ""
                        ) {
                            this.mensajeComponent.setErrorMsg(
                                "Debe completar el nombre en todos los items de V\u00EDnculos a declarar con Funcionarios P\u00FAblicos."
                            );
                            return true;
                        }
                        if (
                            item.CargoFuncionario == null ||
                            item.CargoFuncionario == ""
                        ) {
                            this.mensajeComponent.setErrorMsg(
                                "Debe completar el cargo en todos los items de V\u00EDnculos a declarar con Funcionarios P\u00FAblicos."
                            );
                            return true;
                        }
                        if (item.Vinculo == null || item.Vinculo == "") {
                            this.mensajeComponent.setErrorMsg(
                                "Debe completar el vinculo en todos los items de V\u00EDnculos a declarar con Funcionarios P\u00FAblicos."
                            );
                            return true;
                        }
                    }
                }
            }
            if (!this.codigoDeConducta) {
                this.mensajeComponent.setErrorMsg(
                    "Debe aceptar el C\u00F3digo de Conducta de Proveedores de Molinos agro S.A."
                );
                return true;
            }
        } 

        return false;
    }

    abrirCodigoConducta(siempre: boolean) {
        if (this.codigoConductaVisto == false || siempre) {
            this.codigoConductaVisto = true;
            var filePath =
                location.origin +
                "/Documentacion/CodigoDeConductaParaProveedoresMOA.pdf";
            console.log(filePath);
            var url =
                location.origin +
                "/officetohtml/popup.html?filePath=" +
                filePath;
            this.tryDoctype = url;
            document.getElementById("iframeConducta").setAttribute("src", url);
            document.getElementById("openModalconductaModal").click();
        }
    }

    cargarSolicitudUsuario() {
        this.subscription = this.service.cargarSolicitudUsuario("", this.proveedorId).subscribe(
            (result) => {

                this.CBUNoGranos = result.CBU;

                this.IdSituacionIVA = result.IdSituacionIVA;
                if (this.IdSituacionIVA > 0 && this.IdSituacionIVA != null) {
                    document
                        .getElementById("IdSituacionIVA" + this.IdSituacionIVA.toString())
                        .setAttribute("checked", "true");
                }

                this.IdIngresoBruto = result.IdIngresoBruto;
                if (this.IdIngresoBruto > 0 && this.IdIngresoBruto != null) {
                    document
                        .getElementById("IdIngresoBruto" + this.IdIngresoBruto.toString())
                        .setAttribute("checked", "true");
                }
                
                if (result.VinculoConEmpleadosDeMolinos != null) {
                    console.log("Entramos")
                    this.codigoConductaVisto = true;
                    this.codigoDeConducta = true;

                    this.relacionConEmpleadosChecked =
                        result.VinculoConEmpleadosDeMolinos;
                    if (result.VinculoConEmpleadosDeMolinos) {
                        document
                            .getElementById("radioEmpleadosSi")
                            .setAttribute("checked", "true");
                    } else {
                        document
                            .getElementById("radioEmpleadosNo")
                            .setAttribute("checked", "true");
                    }

                    this.relacionConFuncionariosChecked =
                        result.VinculoConFuncionariosPublicos;
                    if (result.VinculoConFuncionariosPublicos) {
                        document
                            .getElementById("radioFuncionariosSi")
                            .setAttribute("checked", "true");
                    } else {
                        document
                            .getElementById("radioFuncionariosNo")
                            .setAttribute("checked", "true");
                    }

                    this.empleados = result.Empleados;
                    this.funcionarios = result.Funcionarios;
                }


                if (this.esUsuarioCompras && !this.codigoDeConducta)
                {
                    this.esGuardarYNotificar = true;
                }
            },
            (error) => {
                this.mensajeComponent.setErrorMsg(error.message);
            }
        );
    }

    sitIVACheck(id: number) {
        this.IdSituacionIVA = id;
    }
    sitIIBBCheck(id: number) {
        this.IdIngresoBruto = id;
    }

    redirigir() {

        document
            .getElementById("openModalNotificacion")
            .click();
        
        if (this.esUsuarioCompras) {
            this.navService.navegarSeccion(
                "altas"
            );
        } else {
            this.navService.navegarSeccion("/estado-solicitud");
        }
    }
}
