import { EstadoOrdenDeCarga } from "./estadoOrdenDeCarga";
import { OrdenDeCargaCambiosHistorial } from "./ordenDeCargaCambiosHistorial";

export class OrdenDeCarga {
    Id: number;
    CUITTercero: number;
    CUITCliente: number;
    CUITCorredor: number;
    RazonSocialCliente: string;
    NombreChofer: string;
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
    PedidoSAP: string;
    Corredor: string;
    RazonSocialCorredor: string;
    TransporteExiste: boolean;
    ContratoIngresado: string;
    NumeroEntrega: string;
    NumeroPedidoIngresado: string;
    NumeroPedido: string;
    PedidosRespuesta: string;
    ContratosRespuesta: string;
    MensajeValidacionSAP: string;
    ContratoSinCantidadPendiente: boolean;
    DescripcionErrorInterno: string;
    EsOrdenVencida: boolean;
    OrdenDeCargaCambiosHistorial: OrdenDeCargaCambiosHistorial[];
    
    constructor() {
    }


    llenar() {
        this.CUITCliente = 20266044993;
        this.NombreChofer = "Martin Pfeiffer";
        this.CUITChofer = 20391666687;
        this.PatenteAcoplado = "ABC123";
        this.ChasisAcoplado = "ABBSM1231412";
        this.RazonSocialTransporte = "ORLANDI LUIS EDUARDO";
        this.CUITTransporte = 20086452597;
        this.Producto_Id = 1;
        this.Cantidad = 30000;
        this.Observacion = "Comentarios";
        this.ContratoIngresado = "33012251";
    }
}

