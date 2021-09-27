import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { BaseComponent } from '../common/base-components/base-component';
import { SecurityService } from './../common/services/SecurityService';
import { SpinnerSmallComponent } from './../common/view-child/spinner-small/spinner-small.component';
import { NavService } from './../common/services/NavService';
import { FloatMsgService } from './../common/services/FloatMsgService';
import { ModalService } from './../common/services/ModalService';
import { ApikeyService } from './apikey.service';
import { SessionDataService } from '../common/services/SessionDataService';
import { ConfirmationService } from 'primeng/api';

@Component({
    selector: 'app-apikey',
    templateUrl: `apikey.component.html`
})
export class ApikeyComponent extends BaseComponent implements OnInit {

    @ViewChild(SpinnerSmallComponent)
    public spinnerSmallComponent: SpinnerSmallComponent;

    constructor(protected service: ApikeyService, protected navService: NavService, private sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService, private confirmationService: ConfirmationService) {
        super(navService, securityService, floatMsgService, modalService);
        this.spinnerSmallComponent = new SpinnerSmallComponent();
        
        this.apikey = sessionStorage.getItem("apikey");
    }

    checkPermisos() { this.securityService.tienePermisoRedirect("APIKEY"); }

    apikey: string = '';
    
    setTabs() {
        this.setMenuSeccionTab("apikey", "Apikey");
    }

    ngOnInit() {
        // this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList([]);
    }

    ngAfterViewInit(): void {
        
    }

    generarApikey() {
        this.confirmationService.confirm({
            message: '¿Desea generar el api key?',
            accept: () => {
                        this.floatMsgService.setMsgsEmpty();
                this.spinnerSmallComponent.showIt();

                this.unsubscribe();
                try {
                    this.subscription = this.service.generarApikey().subscribe(
                        (result:any) => {
                            this.spinnerSmallComponent.hideIt();
                            if (result.logout == true) {
                                this.sessionDataService.logout();
                            } else if (result.error != undefined && result.error != "") {
                                this.floatMsgService.setErrorMsg(result.error);
                            } else if (result.info != undefined) {
                                this.floatMsgService.setInfoMsg(result.info);
                            } else {
                                this.apikey = result.data;
                                this.sessionDataService.setApikey(result.data);
                                sessionStorage.setItem("apikey", result.data);
                                
                                this.floatMsgService.setSuccessMsg("Se generó correctamente el apikey");
                            }
                            return false;
                        },
                        error => {
                            var errormsj = "Ha ocurrido un error, por favor intentelo nuevamente";
                            this.spinnerSmallComponent.hideIt();
                            this.floatMsgService.setErrorMsg(errormsj);
                        }

                    );
                } catch (e) {
                    this.spinnerSmallComponent.hideIt();
                    this.floatMsgService.setErrorMsg(e);
                    return false; //<-- Prevent Refresh
                }

                return false;
            }
        });

        return false; //<-- Prevent Refresh
    }
}