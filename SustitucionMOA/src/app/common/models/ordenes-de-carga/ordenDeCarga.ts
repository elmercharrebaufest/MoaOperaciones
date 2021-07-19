import { EstadoOrdenDeCarga } from "./estadoOrdenDeCarga";

export class OrdenDeCarga {
    CUITTercero: number;
    CUITCliente: number;
    NombreChofer: string;
    ApellidoChofer: string;
    CUITChofer: number;
    PatenteAcoplado: string;
    ChasisAcoplado: string;
    RazonSocialTransporte: string;
    CUITTransporte: number;
    Producto_Id: number;
    Cantidad: number;
    Observacion: string;
    Estado: EstadoOrdenDeCarga;
    ContratoSAP: string;
    Corredor: string;
    TransporteExiste: boolean;
    ContratoIngresado: string;

    constructor() {
    }


    llenar() {
        this.CUITCliente = 20266044993;
        this.CUITTercero = 20391666689;
        this.NombreChofer = "Martin";
        this.ApellidoChofer = "Pfeiffer";
        this.CUITChofer = 20391666687;
        this.PatenteAcoplado = "ABC123";
        this.ChasisAcoplado = "ABBSM1231412";
        this.RazonSocialTransporte = "ORLANDI LUIS EDUARDO";
        this.CUITTransporte = 20086452597;
        this.Producto_Id = 1;
        this.Cantidad = 30000;
        this.Observacion = "Vamo lo redó";
        this.ContratoIngresado = "33012251";
    }
}

