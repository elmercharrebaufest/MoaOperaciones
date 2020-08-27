import { Component, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { ListBaseComponent } from '../../common/base-components/list-base-component';
import { Archivo } from '../../common/models/archivo';
import { InformeComercial } from '../../common/models/informeComercial';
import { Material } from '../../common/models/material';
import { NuevoAcopio } from '../../common/models/nuevoAcopio';
import { NuevoProduccion } from '../../common/models/nuevoProduccion';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { NavService } from '../../common/services/NavService';
import { SecurityService } from '../../common/services/SecurityService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { MensajeComponent } from '../../common/view-child/mensaje/mensaje.component';
import { SpinnerSmallComponent } from '../../common/view-child/spinner-small/spinner-small.component';
import { EmpresaGranosService } from './empresa-granos.service';

@Component({
    selector: 'app-empresa-granos',
    templateUrl: './app/alta-proveedores/empresa-granos/empresa-granos.component.html',
    styleUrls: ['./app/alta-proveedores/empresa-granos/empresa-granos.component.css', '../Content/css/bootstrap.min.css'],
    providers: [EmpresaGranosService]
})

export class EmpresaGranosComponent extends ListBaseComponent {

    firstFormGroup: FormGroup;
    secondFormGroup: FormGroup;
    http: any;
    fileToUpload: File;
    listaMateriales: Array<Material> = [];
    listaCampanias: any = [];
    campaniaActual: string;

    private newAttributeAlm: NuevoAcopio = new NuevoAcopio();
    private newAttribute: NuevoProduccion = new NuevoProduccion();

    materialesData: any = null;
    CBUSISA: string = "";

    nombreArchivoSeleccionado: string = "";
    fileKeySeleccionado: string = "";
    descripcionSeleccionado: string = "";
    archivoSeleccionado: Archivo = null;

    listaArchivos: Array<Archivo> = [];

    informe = new InformeComercial();

    searchTerm: FormControl = new FormControl();
    myLocalidades = <any>[];

    keyword = 'Nombre';
    data = [];
    autocompleteNotFoundText = "No encontrado";
    
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
                result => {
                    this.data = result;
                },
                error => {
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
                result => {
                    this.data = result;
                },
                error => {
                    this.spinnerSmallComponent.hideIt();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
        }
    }

    constructor(protected service: EmpresaGranosService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
        this.mensajeComponent = new MensajeComponent();
    }

    @ViewChild("msjEmpresaGranos")
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerSmallComponent)
    protected spinnerSmallComponent: SpinnerSmallComponent;

    @ViewChild("spinnerModal")
    protected spinnerModal: SpinnerSmallComponent;

    checkPermisos() { this.securityService.tienePermisoRedirect("ALTA EMPRESA GRANOS"); }

    setTabs() {
        this.setMenuSeccionTab("alta-empresa", "Alta Empresa");
    }

    ngOnInit() {
        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList([]);

        this.obtenerMateriales();
        this.obtenerArchivosSubidos();
        this.obtenerCBUSISA();

        this.addFieldValue();
        this.addFieldValueAlm();
    }

    get email() {
        return this.firstFormGroup.get('email');
    }
    get password() {
        return this.secondFormGroup.get('password');
    }


    handleFileInput(files: FileList, fileKey: string) {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerSmallComponent.showIt();

        this.unsubscribe();
        this.subscription = this.service.postFile(files, fileKey).subscribe(
            result => {
                this.spinnerSmallComponent.hideIt();
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {
                    this.mensajeComponent.setMsgsEmpty();
                    this.mensajeComponent.setSuccessMsg(result.data);
                    this.obtenerArchivosSubidos();
                }
            },
            error => {
                this.spinnerSmallComponent.hideIt();
                this.mensajeComponent.setErrorMsg(error.message);
            }
        );
    }

    obtenerMateriales() {
        this.subscription = this.service.obtenerMateriales().subscribe(
            result => {
                let obj = JSON.parse(result);
                this.listaCampanias = new Array();
                obj.Datos.forEach(element => {
                    let mat = new Material();
                    mat.Id = element.MaterialId;
                    mat.Descripcion = element.Descripcion + " (" + element.CampaniaActual + ")";
                    mat.CampaniaActual = element.CampaniaActual;
                    mat.CampaniaIdActual = element.CampaniaIdActual;
                    this.listaMateriales.push(mat);
                    let cam = {
                        CampaniaActual: element.CampaniaActual,
                        CampaniaIdActual: element.CampaniaIdActual
                    }
                    this.listaCampanias.push(cam);
                });
                const listaCampanias2 = [];
                const map = new Map();
                for (const item of this.listaCampanias) {
                    if (!map.has(item.CampaniaIdActual)) {
                        map.set(item.CampaniaIdActual, true);    // set any value to Map
                        listaCampanias2.push({
                            CampaniaActual: item.CampaniaActual,
                            CampaniaIdActual: item.CampaniaIdActual
                        });
                    }
                }
                this.listaCampanias = listaCampanias2;
            },
            error => {
                this.mensajeComponent.setErrorMsg(error.message);
            }

        );
    }

    obtenerCBUSISA() {
        this.subscription = this.service.obtenerCBUSISA().subscribe(
            result => {
                this.CBUSISA = result;
            },
            error => {
                this.mensajeComponent.setErrorMsg(error.message);
            }
        );
    }

    generarInformeComercial() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerModal.showIt();
        this.unsubscribe();
        this.informe.NuevosCampos.forEach(campo => {
            for (const item of this.listaMateriales) {
                if (item.Id == campo.MaterialId) {
                    campo.CampaniaId = item.CampaniaIdActual;
                }
            }

        });

        this.subscription = this.service.generarInformeComercial(this.informe).subscribe(
            result => {
                this.spinnerModal.hideIt();
                var byteArray = new Uint8Array(result.data);
                var blob = new Blob([byteArray], { type: 'application/pdf' });
                if (window.navigator.msSaveOrOpenBlob) {
                    // IE11
                    window.navigator.msSaveOrOpenBlob(blob, "Informe comercial" + ".pdf");
                } else {
                    var url = window.URL.createObjectURL(blob);
                    var link = document.createElement("a");
                    document.body.appendChild(link);
                    link.href = url;
                    link.download = "Informe comercial" + ".pdf"
                    link.click();
                    setTimeout(function () { window.URL.revokeObjectURL(url); }, 0);

                    return false;
                }

            },
            error => {
                this.spinnerModal.hideIt();
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

        this.subscription = this.service.descargarArchivoSubido(fileKey, archivoID).subscribe(
            result => {
                this.spinnerSmallComponent.hideIt();
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {
                    var byteArray = new Uint8Array(result.FileContents);
                    var blob = new Blob([byteArray], { type: 'application/octet-stream' });

                    if (window.navigator.msSaveOrOpenBlob) {
                        // IE11
                        window.navigator.msSaveOrOpenBlob(blob, result.FileDownloadName);
                    } else {
                        var url = window.URL.createObjectURL(blob);
                        var link = document.createElement("a");
                        document.body.appendChild(link);
                        link.href = url;
                        link.download = result.FileDownloadName;
                        link.click();
                        setTimeout(function () { window.URL.revokeObjectURL(url); }, 0);
                        return false;
                    }
                }
            },
            error => {
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

        this.subscription = this.service.eliminarArchivoSubido(this.fileKeySeleccionado, archivoID).subscribe(
            result => {
                this.spinnerSmallComponent.hideIt();
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {
                    this.obtenerArchivosSubidos();
                    this.fileKeySeleccionado = "";
                    this.nombreArchivoSeleccionado = "";
                    this.descripcionSeleccionado = "";
                    document.getElementById("openModalConfirmModal").click();

                }
            },
            error => {
                this.spinnerSmallComponent.hideIt();
                this.mensajeComponent.setErrorMsg(error.message);
            }
        );
    }

    obtenerArchivosSubidos() {
        this.subscription = this.service.obtenerArchivosSubidos().subscribe(
            result => {
                this.listaArchivos = new Array();

                result.forEach(element => {
                    let archivo = new Archivo();
                    archivo = element;
                    this.listaArchivos.push(archivo);
                });
            },
            error => {
                this.mensajeComponent.setErrorMsg(error.message);
            }
        );
    }

    redirigirAEstado() {
        this.navService.navegarSeccion('/estado-solicitud')
    }

    buscarArchivoPorFileKey(fileKey: string) {
        return this.listaArchivos.find(x => x.FileKey == fileKey).Nombre;
    }

    onSubmit() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerSmallComponent.showIt();
        this.unsubscribe();
        this.subscription = this.service.enviarSolicitud().subscribe(
            result => {
                this.spinnerSmallComponent.hideIt();
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {
                    this.mensajeComponent.setMsgsEmpty();
                    document.getElementById("openModalNotificacion").click();
                }
            },
            error => {
                this.spinnerSmallComponent.hideIt();
                this.mensajeComponent.setErrorMsg(error.message);
            }
        );
    }

    addFieldValue() {
        this.informe.NuevosCampos.push(this.newAttribute)
        this.newAttribute = new NuevoProduccion();
    }

    deleteFieldValue(index) {
        this.informe.NuevosCampos.splice(index, 1);
    }

    addFieldValueAlm() {
        this.informe.NuevosAcopios.push(this.newAttributeAlm)
        this.newAttributeAlm = new NuevoAcopio();
    }

    deleteFieldValueAlm(index) {
        this.informe.NuevosAcopios.splice(index, 1);
    }


    // Comento c�digo que no compila
    //    (function () {
    //    'use strict';
    //    window.addEventListener('load', function () {
    //        // Fetch all the forms we want to apply custom Bootstrap validation styles to
    //        var forms = document.getElementsByClassName('needs-validation');
    //        // Loop over them and prevent submission
    //        var validation = Array.prototype.filter.call(forms, function (form) {
    //            form.addEventListener('submit', function (event) {
    //                if (form.checkValidity() === false) {
    //                    event.preventDefault();
    //                    event.stopPropagation();
    //                }
    //                form.classList.add('was-validated');
    //            }, false);
    //        });
    //    }, false);
    //})();
}

