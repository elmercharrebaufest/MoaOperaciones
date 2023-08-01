import { Component } from "@angular/core";
import { BaseComponent } from "./base-component";
import { OrdenDeCarga } from '../models/ordenes-de-carga/ordenDeCarga';
import { OrdenDeCargaFasonDto } from "../models/ordenes-de-carga-fason/ordenDeCargaFasonDto";

export type Ordenes = OrdenDeCarga | OrdenDeCargaFasonDto

@Component({
    template: ''
})
export class OrdenesBaseComponent extends BaseComponent {
    mensajesOrdenDeCargaFason: Partial<Record<keyof Ordenes, string>> = {};
    mensajesGestionCuit: Partial<Record<keyof Pick<Ordenes, 'CUITIntermediarioFlete'>, string>> = {};
    validando: Partial<Record<keyof Ordenes, boolean>> = {};
    displayModal: keyof Pick<Ordenes, 'CUITIntermediarioFlete'> | null;

    revisarCUITFormatoValido(cuit: string): boolean {
        return cuit && cuit.length == 11 && !Number.isNaN(cuit as unknown as number)
    }

    focusRazonSocialParaGestion = true;
    razonSocialParaGestion = "";
    descripcionIntermediarioFlete = "Llenar en caso que el transporte lo haga un tercero"
}