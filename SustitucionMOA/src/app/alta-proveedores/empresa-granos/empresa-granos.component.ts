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
import { EmpresaGranosService } from "./empresa-granos.service";
import { ContactoComercial } from "../../common/models/contactoComercial";
import { RelacionConEmpleados } from "../../common/models//RelacionConEmpleados";
import { RelacionConFuncionarios } from "../../common/models/relacionConFuncionarios";
import { forEach } from "@angular/router/src/utils/collection";
import { CartaPresentacion } from "../../common/models/cartaPresentacion";

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
    proveedorClasificacion: string = "";

    nombreArchivoSeleccionado: string = "";
    fileKeySeleccionado: string = "";
    descripcionSeleccionado: string = "";
    archivoSeleccionado: Archivo = null;

    listaArchivos: Array<Archivo> = [];

    informe = new InformeComercial();

    cartaPresentacion = new CartaPresentacion();
    private nuevoAtributoCampo: NuevoProduccion = new NuevoProduccion();
    private nuevoAtributoAcompio: NuevoAcopio = new NuevoAcopio();

    searchTerm: FormControl = new FormControl();
    myLocalidades = <any>[];
    mensajeError: string = "";
    tryDoctype: string = "";
    keyword = "Nombre";
    data = [];
    localidades: any = [];
    autocompleteNotFoundText = "No encontrado";

    private selectUndefinedOptionValue:any;
    proveedorId: number = 0;

    estadoSISA: string = "";

    esCorredor: boolean = false;

    esMultiFirma: boolean = false;

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

    @ViewChild("spinnerCartaPresentacion")
    protected spinnerCartaPresentacion: SpinnerSmallComponent;

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

        this.route.params.forEach((params: Params) => {
            if (params["id"] > 0) this.proveedorId = params["id"];
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
        setTimeout(() => {
            var autocompletesLocalidad = document.querySelectorAll('[placeholder="Localidad"]');
            for (let i = 0; i < autocompletesLocalidad.length; i++)
            {
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


    
    selectEventProduccion(item, index) {
        this.informe.NuevosCampos[index].LocalidadId = item.LocalidadId;
    }

    selectEventAlmacenamiento(item, index) {
        this.informe.NuevosAcopios[index].LocalidadID = item.LocalidadId;
    }

    onChangeSearchProduccion(term: string) {
        if (term.length > 2) {
            this.unsubscribe();
            this.subscription = this.service.searchLocalidad(term).subscribe(
                (result) => {
                    this.data = result;
                },
                (error) => {
                    this.spinnerSmallComponent.hideIt();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
        }
    }

    onChangeSearchAlmacenamiento(term: string) {
        if (term.length > 2) {
            this.unsubscribe();
            this.subscription = this.service.searchLocalidad(term).subscribe(
                (result) => {
                    this.data = result;
                },
                (error) => {
                    this.spinnerSmallComponent.hideIt();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
        }
    }

    selectEventInforme(item) {
        this.informe.localidadId = item.LocalidadId;
    }

    onChangeSearchInforme(term: string) {
        if (term.length > 2) {
            this.unsubscribe();
            this.subscription = this.service.searchLocalidad(term).subscribe(
                (result) => {
                    this.data = result;
                },
                (error) => {
                    this.spinnerSmallComponent.hideIt();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
        }
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

    obtenerMateriales() {

        //Sacamos lo de la lista de campaña, ya que ahora son independientes
        this.subscription = this.service.obtenerMateriales().subscribe(
            (result) => {
                let obj = JSON.parse(result);
               // this.listaCampanias = new Array();
                obj.Datos.forEach((element) => {
                    let mat = new Material();
                    mat.Id = element.MaterialId;
                    mat.Descripcion = element.Descripcion;
                    mat.CampaniaActual = element.CampaniaActual;
                    mat.CampaniaIdActual = element.CampaniaIdActual;
                    this.listaMateriales.push(mat);
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
            },
            (error) => {
                this.mensajeComponent.setErrorMsg(error.message);
            }
        );
    }

    obtenerCampanias() {
        this.subscription = this.service.obtenerCampanias().subscribe(
            (result) => {
                let obj = result;
                this.listaCampanias = new Array();
                obj.forEach((element) => {
                    let cam = {
                        CampaniaActual: element.Descripcion,
                        CampaniaIdActual: element.CampaniaId,
                    };
                    this.listaCampanias.push(cam);
                });                

                this.obtenerMateriales();

            },
            (error) => {
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
                },
                (error) => {
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
    }

    generarInformeComercial() {
        this.spinnerModal.showIt();
        this.unsubscribe();
        this.informe.NuevosAcopios.forEach((campo) => {
            campo.CampaniaID = this.campaniaActual;
        });
        this.informe.NuevosCampos.forEach((campo) => {
            campo.CampaniaId = this.campaniaActual;
        });
        this.informe.CampaniaId = this.campaniaActual;
        for (const item of this.listaCampanias) {
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
            .subscribe(
                (result) => {
                    this.spinnerModal.hideIt();
                    if (result.error) {
                        this.mensajeError = result.error;
                    } else {
                        var byteArray = new Uint8Array(result.data);
                        var blob = new Blob([byteArray], {
                            type: "application/pdf",
                        });
                        if (window.navigator.msSaveOrOpenBlob) {
                            // IE11
                            window.navigator.msSaveOrOpenBlob(
                                blob,
                                "Informe comercial" + ".pdf"
                            );
                        } else {
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
                },
                (error) => {
                    this.spinnerModal.hideIt();
                    this.mensajeError = error.message;
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
        } else {
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

    addFieldValue() {
        this.informe.NuevosCampos.push(this.newAttribute);
        this.newAttribute = new NuevoProduccion();
    }

    deleteFieldValue(index) {
        this.informe.NuevosCampos.splice(index, 1);
    }

    addFieldValueAlm() {
        this.informe.NuevosAcopios.push(this.newAttributeAlm);
        this.newAttributeAlm = new NuevoAcopio();
    }

    deleteFieldValueAlm(index) {
        this.informe.NuevosAcopios.splice(index, 1);
    }

    validarInforme() {
        if (this.informe.direccion == "" || !this.informe.direccion) {
            this.mensajeError = "No completo la direccion.";
            return true;
        }
        if (!this.informe.codigoPostal || this.informe.codigoPostal == "") {
            this.mensajeError = "No completo el codigo postal.";
            return true;
        }
        if (
            !this.informe.localidadId ||
            this.informe.localidadId == null ||
            this.informe.localidadId == 0
        ) {
            this.mensajeError =
                "No completo la Localidad en Domicilio Actividad.";
            return true;
        }
        if (
            !this.informe.ContactoComercial.Apellido ||
            this.informe.ContactoComercial.Apellido == ""
        ) {
            this.mensajeError = "No completo el Apellido del contacto.";
            return true;
        }
        if (
            !this.informe.ContactoComercial.Nombres ||
            this.informe.ContactoComercial.Nombres == ""
        ) {
            this.mensajeError = "No completo el Nombre del contacto.";
            return true;
        }
        if (
            !this.informe.ContactoComercial.Puesto ||
            this.informe.ContactoComercial.Puesto == ""
        ) {
            this.mensajeError = "No completo el Puesto del contacto.";
            return true;
        }
        if (
            !this.informe.ContactoComercial.Telefono1 ||
            this.informe.ContactoComercial.Telefono1 == ""
        ) {
            this.mensajeError = "No completo el Telefono del contacto.";
            return true;
        }
        if (this.informe.CampaniaId == 0 || !this.informe.direccion) {
            this.mensajeError = "No completo la Campaña Actual.";
            return true;
        }


        var filaError = 0;

        for (const item of this.informe.NuevosCampos) {
            filaError++;
            if (item.MaterialId == null || item.MaterialId == 0) {
                this.mensajeError = `Debe completar el grano en la fila ${filaError} de capacidad productiva.`;
                return true;
            }
            if (item.LocalidadId == null || item.LocalidadId == 0) {
                this.mensajeError = `Debe completar la localidad en la fila ${filaError} de capacidad productiva.`;
                return true;
            }

            if (item.Hectareas.toString().includes(".") || item.Hectareas.toString().includes(",") || item.Hectareas.toString().includes("e"))
            {
                this.mensajeError = `Las hectareas deben ser un número entero en la fila ${filaError} de capacidad productiva.`;
                return true;
            }
        
            if (item.Toneladas == null || item.Toneladas == 0) {
                this.mensajeError = `Debe completar las toneladas en la fila ${filaError} de capacidad productiva.`;
                return true;
            }

            if (item.Toneladas.toString().includes(".") || item.Toneladas.toString().includes(",") || item.Toneladas.toString().includes("e")) 
            {
                this.mensajeError = `Las toneladas deben ser un número entero en la fila ${filaError} de capacidad productiva.`;
                return true;
            }
        
            if (item.ArrendaPropia == null) {
                this.mensajeError = `Debe completar la condicion en la fila ${filaError} de capacidad productiva.`;
                return true;
            }
        }

        filaError = 0;
        this.informe.NuevosAcopios = this.informe.NuevosAcopios.filter(item => (
            (item.LocalidadID == null || item.LocalidadID == 0) && (item.Toneladas == null || item.Toneladas == 0)) == false);
        //

        for (const item of this.informe.NuevosAcopios) {
            filaError++;
            if (item.LocalidadID == null || item.LocalidadID == 0) {
                this.mensajeError = `Debe completar la localidad en la fila ${filaError} de capacidad planta.`;
                return true;
            }
            
            if (item.Toneladas == null || item.Toneladas == 0) {
                this.mensajeError = `Debe completar las Toneladas en la fila ${filaError} de capacidad planta.`;
                return true;
            }

            if (item.Toneladas.toString().includes(".") || item.Toneladas.toString().includes(",") || item.Toneladas.toString().includes("e"))
            {
                this.mensajeError = `Las toneladas deben ser un número entero en ${filaError} de capacidad productiva.`;
                return true;
            }

            if (item.ArrendaPropia == null) {
                this.mensajeError = `Debe completar la condicion en la fila ${filaError} de capacidad planta.`;
                return true;
            }
        }
        return false;
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
                }
            },
            (error) => {
                this.mensajeComponent.setErrorMsg(error.message);
            }
        );
    }

    agregarCampoCartaPresentacion() {
        this.cartaPresentacion.nuevosCampos.push(this.nuevoAtributoCampo);
        this.nuevoAtributoCampo = new NuevoProduccion();
    }

    borrarCampoCartaPresentacion(index) {
        this.cartaPresentacion.nuevosCampos.splice(index, 1);
    }

    agregarAcopioCartaPresentacion() {
        this.cartaPresentacion.nuevosAcopios.push(this.nuevoAtributoAcompio);
        this.nuevoAtributoAcompio = new NuevoAcopio();
    }
    borrarAcopioCartaPresentacion(index) {
        this.cartaPresentacion.nuevosAcopios.splice(index, 1);
    }

    selectEventCampoCartaPresentacion(item, index) {
        this.cartaPresentacion.nuevosCampos[index].LocalidadId =
            item.LocalidadId;
    }

    selectEventAcopioCartaPresentacion(item, index) {
        this.cartaPresentacion.nuevosAcopios[index].LocalidadID =
            item.LocalidadId;
    }

    onChangeLocalidad(term: string) {
        if (term.length > 2) {
            this.unsubscribe();
            this.subscription = this.service.searchLocalidad(term).subscribe(
                (result) => {
                    this.localidades = result;
                },
                (error) => {
                    this.spinnerSmallComponent.hideIt();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
        }
    }

    generarCartaPresentacion() {
        this.spinnerCartaPresentacion.showIt();
        this.unsubscribe();
        this.cartaPresentacion.nuevosAcopios.forEach((campo) => {
            campo.CampaniaID = this.campaniaActual;
        });
        this.cartaPresentacion.nuevosCampos.forEach((campo) => {
            campo.CampaniaId = this.campaniaActual;
        });

        this.cartaPresentacion.campaniaID = this.campaniaActual;
        this.cartaPresentacion.vendedorActividad = this.proveedorClasificacion;
        

        if (this.validarCartaPresentacion()) {
            this.spinnerCartaPresentacion.hideIt();
            return;
        }

        this.mensajeError = "";
        this.subscription = this.service
            .generarCartaPresentacion(this.cartaPresentacion, this.proveedorId)
            .subscribe(
                (result) => {
                    this.spinnerCartaPresentacion.hideIt();
                    if (result.error) {
                        this.mensajeError = result.error;
                    } else {
                        var byteArray = new Uint8Array(result.data);
                        var blob = new Blob([byteArray], {
                            type: "application/pdf",
                        });
                        if (window.navigator.msSaveOrOpenBlob) {
                            // IE11
                            window.navigator.msSaveOrOpenBlob(
                                blob,
                                "Carta presentación.pdf"
                            );
                        } else {
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
                },
                (error) => {
                    this.spinnerCartaPresentacion.hideIt();
                    this.mensajeError = error.message;
                }
            );
    }

    validarCartaPresentacion() {
        //Corredor
        if (
            this.cartaPresentacion.corredorBolsa == "" ||
            !this.cartaPresentacion.corredorBolsa
        ) {
            this.mensajeError = "No completo el campo bolsa.";
            return true;
        }

        if (
            this.cartaPresentacion.corredorNroRegistro == "" ||
            !this.cartaPresentacion.corredorNroRegistro
        ) {
            this.mensajeError = "No completo el número de registro.";
            return true;
        }

        //Vendedor
          if (
            this.cartaPresentacion.vendedorActividad == "" ||
            !this.cartaPresentacion.vendedorActividad
        ) {
            this.mensajeError = "No seleccionó la actividad.";
            return true;
        }

        if (
            this.cartaPresentacion.vendedorDomicilioFiscal == "" ||
            !this.cartaPresentacion.vendedorDomicilioFiscal
        ) {
            this.mensajeError = "No completo el domicilio fiscal.";
            return true;
        }
      
        if (
            this.cartaPresentacion.vendedorMailContacto == "" ||
            !this.cartaPresentacion.vendedorMailContacto
        ) {
            this.mensajeError = "No completo el mail de contacto.";
            return true;
        }

        if (
            this.cartaPresentacion.vendedorTelefonoContacto == "" ||
            !this.cartaPresentacion.vendedorTelefonoContacto
        ) {
            this.mensajeError = "No completo el teléfono de contacto.";
            return true;
        }

        if (
            this.cartaPresentacion.campaniaID == 0 ||
            !this.cartaPresentacion.campaniaID
        ) {
            this.mensajeError = "No completo la Campaña Actual.";
            return true;
        }

        var filaError = 0;

        for (const item of this.cartaPresentacion.nuevosCampos) {
            filaError++;
            if (item.MaterialId == null || item.MaterialId == 0) {
                this.mensajeError = `Debe completar el grano en la fila ${filaError} de capacidad productiva.`;
                return true;
            }
            if (item.LocalidadId == null || item.LocalidadId == 0) {
                this.mensajeError = `Debe completar la localidad en la fila ${filaError} de capacidad productiva.`;
                return true;
            }
            

            if (item.Hectareas.toString().includes(".") || item.Hectareas.toString().includes(",") || item.Hectareas.toString().includes("e"))
            {
                this.mensajeError = `Las hectareas deben ser un número entero en la fila ${filaError} de capacidad productiva.`;
                return true;
            }

            if (item.Toneladas == null || item.Toneladas == 0) {
                this.mensajeError = `Debe completar las toneladas en la fila ${filaError} de capacidad productiva.`;
                return true;
            }

            if (item.Toneladas.toString().includes(".") || item.Toneladas.toString().includes(",") || item.Toneladas.toString().includes("e"))
            {
                this.mensajeError = `Las toneladas deben ser un número entero en la fila ${filaError} de capacidad productiva.`;
                return true;
            }
            if (item.ArrendaPropia == null) {
                this.mensajeError = `Debe completar la condicion en la fila ${filaError} de capacidad productiva.`;
                return true;
            }
        }

        filaError = 0;
        this.cartaPresentacion.nuevosAcopios = this.cartaPresentacion.nuevosAcopios.filter(item => (
            (item.LocalidadID == null || item.LocalidadID == 0)  && (item.Toneladas == null || item.Toneladas == 0)) == false);

        for (const item of this.cartaPresentacion.nuevosAcopios) {
            if (item.LocalidadID == null || item.LocalidadID == 0) {
                this.mensajeError = `Debe completar la localidad en la fila ${filaError} de capacidad planta.`;
                return true;
            }
            if (item.Toneladas == null || item.Toneladas == 0) {
                this.mensajeError = `Debe completar las Toneladas en la fila ${filaError}  de capacidad planta.`;
                return true;
            }

            if (item.Toneladas.toString().includes(".") || item.Toneladas.toString().includes(",") || item.Toneladas.toString().includes("e"))
            {
                this.mensajeError = `Las toneladas deben ser un número entero en la fila ${filaError} de capacidad planta.`;
                return true;
            }

            if (item.ArrendaPropia == null) {
                this.mensajeError = `Debe completar la condicion en la fila ${filaError} de capacidad planta.`;
                return true;
            }
        }
        return false;
    }
}
