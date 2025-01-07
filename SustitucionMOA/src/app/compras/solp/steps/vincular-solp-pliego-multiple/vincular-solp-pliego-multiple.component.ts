import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Subject, Subscription } from 'rxjs';
import { debounceTime } from 'rxjs/operators';
import { FiltroFechaComponent } from '../../../../common/view-child/filtro-fecha/filtro-fecha.component';
import { PliegoMultipleService } from '../../../pliegoMultiple.service';
import { SolpDto } from './solpDto.interface';

@Component({
    selector: 'vincular-solp-pliego-multiple',
    templateUrl: './vincular-solp-pliego-multiple.component.html',
    styleUrls: ['./vincular-solp-pliego-multiple.component.css']
})
export class VincularSolpPliegoMultipleComponent
    implements OnInit, OnDestroy {

    public numeroSolp: string;

    @ViewChild(FiltroFechaComponent)
    protected filtroFechaComponent: FiltroFechaComponent;

    public creador: string;

    public fiscal: string;

    public sap: boolean;

    public mantenimiento: boolean;

    private debouncer: Subject<void> = new Subject<void>();
    private debouncerSubscription?: Subscription;

    public solps: SolpDto[];

    constructor(protected service: PliegoMultipleService) { }

    ngOnInit() {
        this.debouncerSubscription = this.debouncer
            .pipe(
                debounceTime(500)
            )
            .subscribe(value => {
                this.getSolps();
            });
        this.getSolps();
    }

    ngOnDestroy() {
        if (this.debouncerSubscription) { this.debouncerSubscription.unsubscribe(); }
    }

    public searchParametersChanged() {
        this.debouncer.next();
    }

    public getSolps() {
        console.log({ act: "search here", nroSolp: this.numeroSolp, fecha: this.filtroFechaComponent });
        this.service.getSolpDisponiblesPliegosMultiple(this.numeroSolp, this.filtroFechaComponent.fecha_inicio, this.filtroFechaComponent.fecha_fin, this.creador, this.fiscal, this.sap, this.mantenimiento)
            .subscribe((solps: SolpDto[]) => {
                this.solps = solps;
            });
    }

}
