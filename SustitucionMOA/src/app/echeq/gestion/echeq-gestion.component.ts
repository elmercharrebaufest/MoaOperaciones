import { Component } from '@angular/core';
import { NavService } from '../../common/services/NavService';
import { EcheqService } from '../echeq.service';
import { EcheqFilter } from './echeq.filtros/echeq-filter.model';


@Component({
    selector: 'app-echeq-gestion',
    templateUrl: `echeq-gestion.component.html`,
    providers: [EcheqService]
})
export class EcheqGestionComponent {

    constructor(protected echeqService: EcheqService, protected navService: NavService){}

    setTabs() {
        this.navService.setMenuSeccionTab("echeq", "Gestion");
    }

    ngOnInit(): void {
        let filter = new EcheqFilter();
        //setear fecha por defecto
        //aca 

        this.getContratoPendientePago(filter);
    }

    //buscar tema tabs

    //Aca se me va a llenar la lista de contratos con lo que me devuelve el servicio
    public dataListaCompleta : Array<any> = new Array<any>();

    public getContratoPendientePago(filter: EcheqFilter) {
        this.echeqService.getData(filter.periodo, filter.fechaInicio, filter.fechaFin).subscribe(response => {
            this.dataListaCompleta = response.data;
        });
    }

    //Aplica el filtro a la lista 
    public applyFilter(filter: EcheqFilter){
        this.echeqService.getData(filter.periodo, filter.fechaInicio, filter.fechaFin).subscribe(response => {
            this.dataListaCompleta = response.data;
        });
    }

    public searchData(filter: EcheqFilter){
        console.log("Me llamaron? Estoy aca!!")
        this.getContratoPendientePago(filter);
    }
}