import { Component, OnInit, OnDestroy } from '@angular/core';
import { PliegoDto } from './pliegoDto.interface';
import { ListBaseComponent } from '../../common/base-components/list-base-component';
import { PliegoMultipleService } from '../pliegoMultiple.service';
import { NavService } from '../../common/services/NavService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { SecurityService } from '../../common/services/SecurityService';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { Subject, Subscription } from 'rxjs';
import { debounceTime } from 'rxjs/operators';
import { BlockUI, NgBlockUI } from 'ng-block-ui';

@Component({
    selector: 'app-dashboard-pliego-multiple',
    templateUrl: './dashboard-pliego-multiple.component.html',
    styleUrls: ['./dashboard-pliego-multiple.component.css']
})
export class DashboardPliegoMultipleComponent
    extends ListBaseComponent
    implements OnInit, OnDestroy {

    public nombrePliego?: string = null;

    public pliegos: PliegoDto[];

    private debouncer: Subject<void> = new Subject<void>();
    private debouncerSubscription?: Subscription;

    @BlockUI() blockUI: NgBlockUI;

    constructor(protected service: PliegoMultipleService, protected navService: NavService, protected sessionDataService: SessionDataService,
        protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }

    ngOnInit() {
        this.debouncerSubscription = this.debouncer
            .pipe(
                debounceTime(500)
            )
            .subscribe(value => {
                this.getPliegos();
            });
        this.getPliegos();
    }

    ngOnDestroy() {
        if (this.debouncerSubscription) { this.debouncerSubscription.unsubscribe(); }
    }

    onSearchkeyPress() {
        this.debouncer.next();
    }


    public getPliegos() {
        this.service.getPliegoMultiple(this.nombrePliego).subscribe((pliegos: PliegoDto[]) => {
            this.pliegos = pliegos;
        });
    }

    public eliminarPliego(id: number) {
        this.service.eliminarPliegoMultiple(id).subscribe((result: any) => {
            console.log(result);
            if (result.logout == true) {
                this.sessionDataService.logout();
            } else if (result.error != undefined && result.error != "") {
                this.floatMsgService.setErrorMsg(result.error);
            } else if (result.info != undefined) {
                this.floatMsgService.setInfoMsg(result.info);
            } else {
                this.getPliegos();
            }
        });
    }

    public copiarPliego(id: number): void {
        this.goToSeccion(`/compras/solp/${id}/PLIEGO_MULTIPLE/copy`);
    }

    public editarPliego(id: number): void {
        this.goToSeccion(`/compras/solp/${id}/PLIEGO_MULTIPLE/edit`);
    }



    public generarZipPliego(idSolp) {
        this.blockUI.start('Generando...')
        this.service.descargarZipPliego(idSolp)
            .subscribe(
                (result) => {
                    var byteArray = new Uint8Array(result.FileContents);
                    var blob = new Blob([byteArray], {
                        type: result.ContentType,
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
                        this.blockUI.stop();
                        return false;
                    }
                    this.blockUI.stop();

                },
                (error) => {
                    this.mensajeComponent.setErrorMsg(error.message);
                })
    }
}
