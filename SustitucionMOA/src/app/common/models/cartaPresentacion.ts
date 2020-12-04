import { NuevoProduccion } from "./nuevoProduccion";
import { NuevoAcopio } from "./nuevoAcopio";

export class CartaPresentacion {
    /*
    public cuitVendedor: string;
    public razonSocialVendedor: string;
    public actividadID: number;
    public actividad: string;
    public campaniaID: number;

    public antiguedadActividad: string;
    public antecedentesComerciales: string;

    public mailContacto: string;

    public telefonoContacto: string;
    */

    public corredorBolsa: string;
    public corredorNroRegistro: string;

    public vendedorCuit: string;
    public vendedorRazonSocial: string;
    public vendedorDomicilioFiscal: string;
    public vendedorActividad: string;

    public campaniaID: number;

    public vendedorAntiguedadEnActividad: string;
    public vendedorAntecedentesComerciales: string;
    public vendedorMailContacto: string;
    public vendedorTelefonoContacto: string;
    public vendedorDomicilioReal: string;
    public vendedorCosecha: string;


    public nuevosCampos: NuevoProduccion[];
    public nuevosAcopios: NuevoAcopio[]; 

    constructor() {
        this.nuevosCampos = new Array<NuevoProduccion>();
        this.nuevosAcopios = new Array<NuevoAcopio>();
    }
}
