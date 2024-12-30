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
}
