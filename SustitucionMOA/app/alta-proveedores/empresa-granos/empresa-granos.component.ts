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


    onSubmit() {
    }
}
