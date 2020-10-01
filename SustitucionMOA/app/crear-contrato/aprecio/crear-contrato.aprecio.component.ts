import { Component, OnInit, ViewChild } from '@angular/core';
import { CrearContratoBaseComponent } from './../crear-contrato.component';
import { CrearContratoService, CrearContratoAPrecioService } from './../crear-contrato.service';
import { ContratoAPrecio } from "../../common/models/contratoAPrecio";

@Component({
    selector: 'app-crear-contrato-aprecio',
    templateUrl: `./app/crear-contrato/aprecio/crear-contrato.aprecio.component.html?v=${new Date().getTime()}`,
    providers: [{ provide: CrearContratoService, useClass: CrearContratoAPrecioService }]
})
export class CrearContratoAPrecioComponent extends CrearContratoBaseComponent {

    contrato: ContratoAPrecio = new ContratoAPrecio();
    localidades = [];
    keyword = 'Nombre';
    autocompleteNotFoundText = "No encontrado";

    setTabs() {
        this.setMenuSeccionTab("crear-contrato", "A Precio");
    }

    selectEventLocalidad(item) {
        this.contrato.LocalidadId = item.LocalidadId;
    }

    onChangeSearchLocalidad(term: string) {
        if (term.length > 2) {
            this.unsubscribe();
            this.subscription = this.service.searchLocalidad(term).subscribe(
                result => {
                    this.localidades = result;
                },
                error => {                   
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
        }
    }

    onBoletoSelected() {
        this.contrato.BolsaId = 0;        
        if (this.contrato.BoletoId == 1) {
            this.bolsasSelect = this.bolsasConfirma;
        }
        if (this.contrato.BoletoId == 2) {
            this.bolsasSelect = this.bolsasFisico;
        } 
        if (this.contrato.BoletoId == 3) {
            this.bolsasSelect = [];
        }
        if (this.contrato.BoletoId == 4) {
            this.bolsasSelect = this.bolsasCarta;
        } 
    }

}