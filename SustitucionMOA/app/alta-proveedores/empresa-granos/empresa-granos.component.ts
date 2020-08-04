import { Component } from '@angular/core';
import { SecurityService } from '../../common/services/SecurityService';
import { NavService } from '../../common/services/NavService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { ModalService } from '../../common/services/ModalService';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { BaseService } from '../../common/services/BaseService';
import { BaseComponent } from '../../common/base-components/base-component';
import { MensajeComponent } from '../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from '../../common/view-child/spinner/spinner.component'
import { EmpresaGranosService } from './empresa-granos.service';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { ListBaseComponent } from '../../common/base-components/list-base-component';

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

    EmplRelDep: string;
    EmplRelDepCant: string;
    Rodados: string;
    RodadosOtros: string;
    Chacra: string;
    ChacraOtros: string;
    AntigActividad: string;
    ActuacionProd: string;
    ClienteAnt: string;
    Comentarios: string;
    Domicilio: string;

    constructor(protected service: EmpresaGranosService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }

    ngOnInit() {
        this.setTabs();
        this.navService.setSeccionList([]);
        this.firstFormGroup = new FormGroup({
            // email: new FormControl('', [Validators.required, Validators.email])
        });
        this.secondFormGroup = new FormGroup({
            password: new FormControl('', Validators.required)
        });
    }

    get email() {
        return this.firstFormGroup.get('email');
    }
    get password() {
        return this.secondFormGroup.get('password');
    }

    handleFileInput(files: FileList, fileKey: string) {
        //this.mensajeComponent.setMsgsEmpty();
        //this.spinnerSmallComponent.showIt();


        this.service.postFile(files, fileKey)
     
        return false; 

        /*
        this.unsubscribe();
        this.subscription = this.service.postFile(files.item(0), fileName).subscribe(
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
                    this.mensajeComponent.setInfoMsg("Documento guardado correctamente");
                }
            },
            error => {
                this.spinnerSmallComponent.hideIt();
                this.mensajeComponent.setErrorMsg(error.message);
            }
        );*/

    }



    generarInformeComercial() {
        //this.mensajeComponent.setMsgsEmpty();
        //this.spinnerSmallComponent.showIt();
        this.unsubscribe();
        this.subscription = this.service.generarInformeComercial(this.EmplRelDep, this.EmplRelDepCant, this.Rodados, this.RodadosOtros, this.Chacra, this.ChacraOtros, this.AntigActividad, this.ActuacionProd, this.ClienteAnt, this.Comentarios, this.Domicilio).subscribe(
            result => {
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
                    link.download = "Informe comercial"  + ".pdf"
                    link.click();
                    setTimeout(function () { window.URL.revokeObjectURL(url); }, 0);
                    return false;
                }
                
            },
            error => {
                console.log(error.message);
            }
        );
        return false;  // <- Prevent href del a
    }

    onSubmit() {
        var modal = document.getElementById("modal");
        var container = document.getElementById("container");
        modal.className = " show";
        container.className += "hidden"
    }
}

