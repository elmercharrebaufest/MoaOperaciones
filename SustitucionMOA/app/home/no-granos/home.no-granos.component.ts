import { Component, OnInit, ViewChild } from '@angular/core';
import { HomeService, HomeNGService } from './../home.service';
import { HomeComponent } from './../home.component';
import { NavService } from './../../common/services/NavService';
import { MensajeComponent } from './../../common/view-child/mensaje/mensaje.component';

@Component({
    selector: 'my-app',
    //template: '<h1>{{titulo}}</h1>'
    templateUrl: `./app/home/no-granos/home.no-granos.component.html?v=${new Date().getTime()}`,
    providers: [{ provide: HomeService, useClass: HomeNGService }]
})
export class HomeNGSComponent extends HomeComponent {

    setTabs() {
        this.setMenuSeccionTab("home-ngs", "");
        this.sessionDataService.setGranosSelected("N");
        sessionStorage.setItem("granosSelected", "N");
    }

    ngOnInit() {
        if (this.filtroFechaComponent.getPeriodo() == "1") {
            this.filtroFechaComponent.setPeriodoInitial("2");
        }
        super.ngOnInit();
    }

    checkPermisos() {
        if (this.securityService.esNoGranosRedirect()) {
            this.securityService.tienePermisoRedirect("CONSULTAR HOME NG");
        }
    }

    showModalTableResponsive(cuentaCorriente: any) {
        this.modalService.openModalTableResponsive("Movimientos", [
            { etiqueta: "F. Emisión", valor: cuentaCorriente.docDate },
            { etiqueta: "F. Vto.", valor: cuentaCorriente.fecVto },
            { etiqueta: "Nº Cte.", valor: cuentaCorriente.docNo },
            { etiqueta: "Descripción", valor: cuentaCorriente.descripcion },
            { etiqueta: "Contrato", valor: cuentaCorriente.contrato },
            { etiqueta: "Moneda", valor: cuentaCorriente.moneda },
            { etiqueta: "TC", valor: cuentaCorriente.ukursString },
            { etiqueta: "Debe", valor: cuentaCorriente.debeString },
            { etiqueta: "Haber", valor: cuentaCorriente.haberString }
        ]);
        return false;
    }
}