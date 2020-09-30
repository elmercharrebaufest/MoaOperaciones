import { Component } from '@angular/core';
import { ListBaseComponent } from '../../common/base-components/list-base-component';
import { Archivo } from '../../common/models/archivo';
import { CartaPresentacion } from '../../common/models/cartaPresentacion';
import { Material } from '../../common/models/material';
import { NuevoAcopio } from '../../common/models/nuevoAcopio';
import { NuevoProduccion } from '../../common/models/nuevoProduccion';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { NavService } from '../../common/services/NavService';
import { SecurityService } from '../../common/services/SecurityService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { EmpresaGranosService } from '../empresa-granos/empresa-granos.service';

@Component({
    selector: 'app-empresa-corredor',
    templateUrl: './app/alta-proveedores/empresa-corredor/empresa-corredor.component.html',
    styleUrls: ['./app/alta-proveedores/empresa-corredor/empresa-corredor.component.css', '../Content/css/bootstrap.min.css'],
    providers: [EmpresaGranosService]
})
export class EmpresaCorredorComponent extends ListBaseComponent {

    cartaPresentacion = new CartaPresentacion();
    private nuevoAtributoCampo: NuevoProduccion = new NuevoProduccion();
    private nuevoAtributoAcompio: NuevoAcopio = new NuevoAcopio();

    listaCampanias: any = [];
    listaMateriales: Array<Material> = [];
    localidades: any = [];
    listaArchivos: Array<Archivo> = [];

    nombreArchivoSeleccionado: string = "";
    fileKeySeleccionado: string = "";
    descripcionSeleccionado: string = "";
    archivoSeleccionado: Archivo = null;

    mensajeError: string = "";

    data = [];


    constructor(protected service: EmpresaGranosService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }

    ngOnInit() {
        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList([]);

        this.obtenerMateriales();
        this.obtenerArchivosSubidos();
        //this.obtenerInfoProveedor();

        this.agregarCampo()
        this.agregarAcopio()
    }

    agregarCampo() {
        this.cartaPresentacion.nuevosCampos.push(this.nuevoAtributoCampo)
        this.nuevoAtributoCampo = new NuevoProduccion();
    }

    borrarCampo(index) {
        this.cartaPresentacion.nuevosCampos.splice(index, 1);
    }

    agregarAcopio() {
        this.cartaPresentacion.nuevosAcopios.push(this.nuevoAtributoAcompio)
        this.nuevoAtributoAcompio = new NuevoAcopio();
    }
    borrarAcopio(index) {
        this.cartaPresentacion.nuevosAcopios.splice(index, 1);
    }

    selectEventCampo(item, index) {
        console.log(item)
        console.log(index)
        this.cartaPresentacion.nuevosCampos[index].LocalidadId = item.LocalidadId;
    }

    selectEventAcopio(item, index) {
        this.cartaPresentacion.nuevosAcopios[index].LocalidadID = item.LocalidadId;
    }


    onChangeLocalidad(term: string) {
        if (term.length > 2) {
            this.unsubscribe();
            this.subscription = this.service.searchLocalidad(term).subscribe(
                result => {
                    this.data = result;
                    console.log(this.data)

                },
                error => {
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

    }

    obtenerMateriales() {
        this.subscription = this.service.obtenerMateriales().subscribe(
            result => {
                let obj = JSON.parse(result);
                this.listaCampanias = new Array();
                obj.Datos.forEach(element => {
                    let mat = new Material();
                    mat.Id = element.MaterialId;
                    mat.Descripcion = element.Descripcion;
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

        this.subscription = this.service.eliminarArchivoSubido(archivoID).subscribe(
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
}
