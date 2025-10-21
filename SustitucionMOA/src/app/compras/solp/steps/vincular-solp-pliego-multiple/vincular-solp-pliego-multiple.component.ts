import { Component, EventEmitter, Input, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { SelectItem } from 'primeng/api';
import { Subject, Subscription } from 'rxjs';
import { debounceTime } from 'rxjs/operators';
import { FiltroFechaComponent } from '../../../../common/view-child/filtro-fecha/filtro-fecha.component';
import { ComprasService } from '../../../compras.service';
import { PliegoMultipleService } from '../../../pliegoMultiple.service';
import { SolpDto } from './solpDto.interface';
import { SecurityService } from '../../../../common/services/SecurityService';

@Component({
    selector: 'vincular-solp-pliego-multiple',
    templateUrl: './vincular-solp-pliego-multiple.component.html',
    styleUrls: ['./vincular-solp-pliego-multiple.component.css']
})
export class VincularSolpPliegoMultipleComponent
    implements OnInit, OnDestroy {

    @Output()
    public solpSeleccionadaListChanged = new EventEmitter<number[]>();

    @Output()
    public solpDtoSeleccionadaListChanged = new EventEmitter<SolpDto[]>();


    @Input()
    public pliegoId: number | null;

    @Input()
    public existingCheckedSolps: SolpDto[] = [];

    public numeroSolp: string;
    public nombrePliego: string;

    @ViewChild(FiltroFechaComponent)
    protected filtroFechaComponent: FiltroFechaComponent;

    public creador: string;
    public creadores: SelectItem[] = [];

    public fiscal: string;
    public fiscales: SelectItem[] = [];

    public incluirGuardadas: boolean = true;
    public sap: boolean;
    public mantenimiento: boolean;
    public web: boolean;
    public repoAutomatica: boolean;
    public contratoMarco: boolean;

    private debouncer: Subject<void> = new Subject<void>();
    private debouncerSubscription?: Subscription;

    @BlockUI() blockUI: NgBlockUI;

    public solps: SolpDto[];

    constructor(protected service: PliegoMultipleService, protected comprasService: ComprasService, private securityService: SecurityService) { }

    ngOnInit() {
        this.cargarFiltrosUsuario();

        this.createDebouncerAndSubscribe();
    }

    private createDebouncerAndSubscribe() {
        this.debouncerSubscription = this.debouncer
            .pipe(
                debounceTime(500)
            )
            .subscribe(value => {
                this.getSolps();
            });
        this.debouncer.next(); // launch first search
    }

    private cargarFiltrosUsuario() {
        this.comprasService.listarUsuarioCreadorSolp().subscribe((result: any) => {
            result.data.forEach(x =>
                x.filter((d: { Id: number; }) => d.Id !== 0)
                    .forEach((d: { Id: number; Mail: string; }) => this.creadores.push({
                        label: d.Mail, value: d.Id
                    }))
            );
        });

        this.comprasService.listarFiscalesSolp().subscribe((result: any) => {
            result.data.forEach((x: string) => this.fiscales.push({
                label: x, value: x
            }));
        });
    }

    ngOnDestroy() {
        if (this.debouncer) { this.debouncer.complete(); }
        if (this.debouncerSubscription) { this.debouncerSubscription.unsubscribe(); }
        this.solpSeleccionadaListChanged.complete();
        this.solpDtoSeleccionadaListChanged.complete();
    }

    public searchParametersChanged() {
        this.debouncer.next();
    }

    public notifyChange() {
        this.solpSeleccionadaListChanged.emit(this.solps.filter(x => x.Selected).map(x => x.Id));
        this.solpDtoSeleccionadaListChanged.emit(this.solps.filter(x => x.Selected));
        this.existingCheckedSolps = (this.solps || []).filter(x => x.Selected);
    }

    public getSolps() {
        try {
            this.blockUI.start("Cargando");
            let organizacionDeCompraId:string;

            if(this.securityService.tienePermiso("COMPRAS RRHH")){
                organizacionDeCompraId = "4010";
            }
            else{
                organizacionDeCompraId = "2029";
            }

            this.service.getSolpDisponiblesPliegosMultiple(this.numeroSolp, this.nombrePliego, this.filtroFechaComponent.fecha_inicio, this.filtroFechaComponent.fecha_fin, this.creador, this.fiscal, this.sap, this.mantenimiento, this.web, this.repoAutomatica, this.contratoMarco, this.pliegoId, this.incluirGuardadas, organizacionDeCompraId)
                .subscribe(
                    (result: any) => {
                        if (result.logout == true) {
                            //this.sessionDataService.logout();
                        } else if (result.error != undefined && result.error != "") {
                            //this.floatMsgService.setErrorMsg(result.error);
                            alert(result.error);
                        } else if (result.info != undefined) {
                            //this.floatMsgService.setInfoMsg(result.info);
                            alert(result.info);
                        } else {
                            let solps = result as SolpDto[] | null;
                            if (solps) {
                                const newSolps = solps.filter(newSolp => !(this.existingCheckedSolps || []).some(existingSolp => existingSolp.Id === newSolp.Id));
                                this.solps = [...this.existingCheckedSolps, ...newSolps];

                            } else {
                                this.solps = this.solps || [];
                            }
                            this.incluirGuardadas = false;
                            this.blockUI.stop();
                        }
                    },
                    () => {
                        this.incluirGuardadas = false;
                        this.blockUI.stop();
                    }
                );
        } catch (e) {
            //this.floatMsgService.setErrorMsg(e);
            alert(e);
            this.blockUI.stop();

            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    }
}
