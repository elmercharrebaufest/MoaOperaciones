import { Component, ViewChild } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";
import { ActivatedRoute, Params } from "@angular/router";
import { ListBaseComponent } from "../../common/base-components/list-base-component";
import { Archivo } from "../../common/models/archivo";
import { InformeComercial } from "../../common/models/informeComercial";
import { Material } from "../../common/models/material";
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

@Component({
    selector: "app-empresa-granos",
    templateUrl: "empresa-granos.component.html",
    styleUrls: [
        "empresa-granos.component.css",
        "../../../../Content/css/bootstrap.min.css",
    ],
    providers: [EmpresaGranosService],
})
export class EmpresaGranosComponent extends ListBaseComponent {
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
    relacionConEmpleadosChecked: boolean = null;
    relacionConFuncionariosChecked: boolean = null;
    codigoConductaVisto: boolean = false;
    codigoDeConducta: boolean = false;
    materialesData: any = null;
    CBUSISA: string = "";
    proveedorCUIT: string = "";
    razonSocial: string = "";
    proveedorClasificacion: string = "";
    altaInterna: boolean = false;
    observacion: string = "";

    nombreArchivoSeleccionado: string = "";
    fileKeySeleccionado: string = "";
    descripcionSeleccionado: string = "";
    archivoSeleccionado: Archivo = null;

    listaArchivos: Array<Archivo> = [];
    granosFlag = sessionStorage.getItem("granosFlag");

    informe = new InformeComercial();

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
    proveedorMail: string;

    estadoSISA: string = "";

    esCorredor: boolean = false;

    esMultiFirma: boolean = false;

    esGuardarYNotificar: boolean = false;
    esUsuarioComercial: boolean = false;
    puedeAltaInterna: boolean = this.isAuthorized('ALTA INTERNA GRANOS');

    constructor(
        protected service: EmpresaGranosService,
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

    checkPermisos() {
        this.securityService.tienePermisoRedirect("ALTA EMPRESA GRANOS");
    }

    setTabs() {
        this.setMenuSeccionTab("alta-empresa", "Alta Empresa");
    }

    ngOnInit() {
        this.esCorredor = sessionStorage.getItem("tipoUsuario") === "CORR";

        this.esMultiFirma = this.securityService.tienePermiso(
            "CONSULTAR VENDEDOR PENDIENTES"
        );

        //comercial
        this.esUsuarioComercial =  this.securityService.tienePermiso('ABM EMPRESAS');

        this.route.params.forEach((params: Params) => {
            if (params["id"] > 0) this.proveedorId = params["id"];
        });

        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList([]);

        this.obtenerArchivosSubidos();
        this.cargarSolicitudUsuario();
        this.obtenerInfoProveedor();

        //Agarramos los input que son de autocomplete de localidad (que usan un componente aparte) y les ponemos en off el autocomplete de chrome, para que no rellene formularios
        setTimeout(() => {
            var autocompletesLocalidad = document.querySelectorAll('[placeholder="Localidad"]');
            for (let i = 0; i < autocompletesLocalidad.length; i++) {
                autocompletesLocalidad[i].setAttribute("autocomplete", "chrome-off");
            }
        }, 1000);

        this.setTipoNotificacion();

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
                    this.CBUSISA = result.ProveedorCBU;
                    this.estadoSISA = result.EstadoSISA;
                    this.proveedorClasificacion = result.ProveedorClasificacion;
                    this.proveedorCUIT = result.ProveedorCUIT;
                    this.razonSocial = result.RazonSocial;
                    this.altaInterna = result.AltaInterna;
                    this.proveedorMail = result.Mail;

                    if(this.puedeAltaInterna && result.Observacion != null){
                        this.mensajeComponent.setInfoMsg("Observación: " + result.Observacion);
                    }
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

    redirigirAEstado() {
        if (this.esMultiFirma) {
            this.navService.navegarSeccion(
                "/dato-fiscal/vendedores-pendientes"
            );
        }
        else if (this.esUsuarioComercial)
        {
            this.navService.navegarSeccion(
                "/altas"
            );
        }
        else {
            this.navService.navegarSeccion("/estado-solicitud");
        }
    }

    buscarArchivoPorFileKey(fileKey: string) {
        return this.listaArchivos.find((x) => x.FileKey == fileKey).Nombre;
    }

    onSubmit() {
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

    validarTyC() {
        if (this.relacionConEmpleadosChecked == null) {
            this.mensajeComponent.setErrorMsg(
                "Debe completar Vínculos a declarar con Empleados de Molinos agro S.A."
            );
            return true;
        }
        if (this.relacionConEmpleadosChecked == false) {
            this.empleados = [];
        }
        if (this.relacionConEmpleadosChecked == true) {
            if (this.empleados.length == 0) {
                this.mensajeComponent.setErrorMsg(
                    "Debe completar Vínculos a declarar con Empleados de Molinos agro S.A."
                );
            } else {
                for (const item of this.empleados) {
                    if (
                        item.NombreProveedora == null ||
                        item.NombreProveedora == ""
                    ) {
                        this.mensajeComponent.setErrorMsg(
                            "Debe completar el nombre en todos los items de Vínculos a declarar con Empleados de Molinos agro."
                        );
                        return true;
                    }
                    if (
                        item.CargoProveedora == null ||
                        item.CargoProveedora == ""
                    ) {
                        this.mensajeComponent.setErrorMsg(
                            "Debe completar el cargo en todos los items de Vínculos a declarar con Empleados de Molinos agro."
                        );
                        return true;
                    }
                    if (
                        item.NombreMolinos == null ||
                        item.NombreMolinos == ""
                    ) {
                        this.mensajeComponent.setErrorMsg(
                            "Debe completar el nombre en todos los items de Vínculos a declarar con Empleados de Molinos agro."
                        );
                        return true;
                    }
                    if (item.Vinculo == null || item.Vinculo == "") {
                        this.mensajeComponent.setErrorMsg(
                            "Debe completar el vinculo en todos los items de Vínculos a declarar con Empleados de Molinos agro."
                        );
                        return true;
                    }
                }
            }
        }

        if (this.relacionConFuncionariosChecked == null) {
            this.mensajeComponent.setErrorMsg(
                "Debe completar Vínculos a declarar con Funcionarios Públicos"
            );
            return true;
        }
        if (this.relacionConFuncionariosChecked == false) {
            this.funcionarios = [];
        }
        if (this.relacionConFuncionariosChecked == true) {
            if (this.funcionarios.length == 0) {
                this.mensajeComponent.setErrorMsg(
                    "Debe completar Vínculos a declarar con Funcionarios Públicos"
                );
            } else {
                for (const item of this.funcionarios) {
                    if (item.NombreFirma == null || item.NombreFirma == "") {
                        this.mensajeComponent.setErrorMsg(
                            "Debe completar el nombre en todos los items de Vínculos a declarar con Funcionarios Públicos."
                        );
                        return true;
                    }
                    if (item.CargoFirma == null || item.CargoFirma == "") {
                        this.mensajeComponent.setErrorMsg(
                            "Debe completar el cargo en todos los items de Vínculos a declarar con Funcionarios Públicos."
                        );
                        return true;
                    }
                    if (
                        item.NombreFuncionario == null ||
                        item.NombreFuncionario == ""
                    ) {
                        this.mensajeComponent.setErrorMsg(
                            "Debe completar el nombre en todos los items de Vínculos a declarar con Funcionarios Públicos."
                        );
                        return true;
                    }
                    if (
                        item.CargoFuncionario == null ||
                        item.CargoFuncionario == ""
                    ) {
                        this.mensajeComponent.setErrorMsg(
                            "Debe completar el cargo en todos los items de Vínculos a declarar con Funcionarios Públicos."
                        );
                        return true;
                    }
                    if (item.Vinculo == null || item.Vinculo == "") {
                        this.mensajeComponent.setErrorMsg(
                            "Debe completar el vinculo en todos los items de Vínculos a declarar con Funcionarios Públicos."
                        );
                        return true;
                    }
                }
            }
        }
        if (this.codigoDeConducta == false) {
            this.mensajeComponent.setErrorMsg(
                "Debe aceptar el Código de Conducta de Proveedores de Molinos agro S.A."
            );
            return true;
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
                if (result.VinculoConEmpleadosDeMolinos != null) {
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

                    this.setTipoNotificacion();
                }
            },
            (error) => {
                this.mensajeComponent.setErrorMsg(error.message);
            }
        );
    }




    submitAltaInterna() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerSmallComponent.showIt();
        var datos = {
            Empleados: this.empleados,
            Funcionarios: this.funcionarios,
            VinculoConEmpleadosDeMolinos: this.relacionConEmpleadosChecked,
            VinculoConFuncionariosPublicos: this.relacionConFuncionariosChecked,
        };
        this.unsubscribe();
        this.subscription = this.service
            .SolicitudAltaInterna(this.proveedorId, datos)
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
                        this.navService.navegarSeccion(
                            "/altas"
                        );            
                    }
                },
                (error) => {
                    this.spinnerSmallComponent.hideIt();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );

    }

    setTipoNotificacion() {
        if (this.esUsuarioComercial) {
            this.esGuardarYNotificar = this.codigoDeConducta;
            return;
        }

        this.esGuardarYNotificar = true;
    }

}
