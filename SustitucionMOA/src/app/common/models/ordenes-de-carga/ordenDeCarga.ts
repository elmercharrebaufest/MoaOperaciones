import { EstadoOrdenDeCarga } from "./estadoOrdenDeCarga";

export class OrdenDeCarga  {
    CUITTercero: number;
    CUITCliente: number;
    NombreChofer: string;
    ApellidoChofer: string;
    CUITChofer: number;
    PatenteAcoplado: string;
    ChasisAcoplado: string;
    RazonSocialTransporte: string;
    CUITTransporte: number;
    Producto: string;
    Cantidad: number;
    Observacion: string;
    Estado: EstadoOrdenDeCarga;
    ContratoSAP: string;
    Corredor: string;
    TransporteExiste: boolean;

    constructor() {
    }


    llenar() {
        this.CUITCliente = 30765460163;
        this.CUITTercero = 20391666689;
        this.NombreChofer = "Martin";
        this.ApellidoChofer = "Pfeiffer";
        this.CUITChofer = 20391666687;
        this.PatenteAcoplado = "ABC123";
        this.ChasisAcoplado = "ABBSM1231412";
        this.RazonSocialTransporte = "Martin";
        this.CUITTransporte = 20391666686;
        this.Producto = "Soja";
        this.Cantidad = 30000;
        this.Observacion = "Vamo lo redó";
    }
}

