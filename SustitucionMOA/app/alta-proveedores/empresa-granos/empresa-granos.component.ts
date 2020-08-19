import { Component, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { ListBaseComponent } from '../../common/base-components/list-base-component';
import { InformeComercial } from '../../common/models/informeComercial';
import { Material } from '../../common/models/material';
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
    campaniaActual: string;

    private fieldArray: Array<any> = [];
    private newAttribute: any = {};
    private fieldArrayAlm: Array<any> = [];
    private newAttributeAlm: any = {};

    materialesData: any = null;

    nombreArchivoInformeComercialFirmado: string = "";
    nombreArchivoConstanciaCBU: string = "";
    nombreArchivoConstanciaCBUMercaderia: string = "";
    nombreArchivoConstanciaCUIT: string = "";
    nombreArchivoInscripcionIIBB: string = "";
    nombreArchivoCertificadoExclusionIVA: string = "";
    nombreArchivoCertificadoExclusionIIBB: string = "";
    nombreArchivoCertificadoExclusionSUSS: string = "";
    nombreArchivoCertificadoExclusionGanancias: string = "";
    nombreArchivoSIPER: string = "";
    nombreArchivoDocumentacionEnBolsa: string = "";


    informe = new InformeComercial();

    searchTerm: FormControl = new FormControl();
    myLocalidades = <any>[];


    constructor(protected service: EmpresaGranosService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
        this.mensajeComponent = new MensajeComponent();
    }

    @ViewChild("msjEmpresaGranos")
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerSmallComponent)
    protected spinnerSmallComponent: SpinnerSmallComponent;


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
        /*this.searchTerm.valueChanges.subscribe(
            term => {
                if (term != '') {
                    this.service.searchLocalidad(term).subscribe(
                        data => {
                            this.myLocalidades = data as any[];
                            //console.log(data[0].BookName);
                        })
                }
            })*/

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
                obj.Datos.forEach(element => {
                    let mat = new Material();
                    mat.Id = element.MaterialId;
                    mat.Descripcion = element.Descripcion;
                    this.campaniaActual = element.CampaniaTablero;
                    this.listaMateriales.push(mat);
                });
            },
            error => {
                this.mensajeComponent.setErrorMsg(error.message);
            }

        );
    }

    generarInformeComercial() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerSmallComponent.showIt();
        this.unsubscribe();
        this.subscription = this.service.generarInformeComercial(this.informe).subscribe(
            result => {
                this.spinnerSmallComponent.hideIt();
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
                this.spinnerSmallComponent.hideIt();
                this.mensajeComponent.setErrorMsg(error.message);
            }
        );
    }

    descargarArchivo(fileKey: string) {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerSmallComponent.showIt();
        this.unsubscribe();
        this.subscription = this.service.descargarArchivoSubido(fileKey).subscribe(
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

    obtenerArchivosSubidos() {
        this.subscription = this.service.obtenerArchivosSubidos().subscribe(
            result => {
                this.nombreArchivoInformeComercialFirmado = result.informeComercialFirmado;
                this.nombreArchivoConstanciaCBU = result.constanciaCBU;
                this.nombreArchivoConstanciaCBUMercaderia = result.constanciaCBUMercaderia;
                this.nombreArchivoConstanciaCUIT = result.constanciaCUIT;
                this.nombreArchivoInscripcionIIBB = result.inscripcionIIBB;
                this.nombreArchivoCertificadoExclusionIVA = result.certificadoExclusionIVA;
                this.nombreArchivoCertificadoExclusionIIBB = result.certificadoExclusionIIBB;
                this.nombreArchivoCertificadoExclusionSUSS = result.certificadoExclusionSUSS;
                this.nombreArchivoCertificadoExclusionGanancias = result.certificadoExclusionGanancias;
                this.nombreArchivoSIPER = result.SIPER;
                this.nombreArchivoDocumentacionEnBolsa = result.documentacionEnBolsa;
            },
            error => {
                this.mensajeComponent.setErrorMsg(error.message);
            }
        );
    }

    redirigirAEstado() {
        this.navService.navegarSeccion('/estado-solicitud')
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
        this.fieldArray.push(this.newAttribute)
        this.newAttribute = {};
    }

    deleteFieldValue(index) {
        this.fieldArray.splice(index, 1);
    }

    addFieldValueAlm() {
        this.fieldArrayAlm.push(this.newAttributeAlm)
        this.newAttributeAlm = {};
    }

    deleteFieldValueAlm(index) {
        this.fieldArrayAlm.splice(index, 1);
    }
}

