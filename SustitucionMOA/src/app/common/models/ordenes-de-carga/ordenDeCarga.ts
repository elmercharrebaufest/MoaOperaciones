import { EstadoOrdenDeCarga } from "./estadoOrdenDeCarga";
import { OrdenDeCargaCambiosHistorial } from "./ordenDeCargaCambiosHistorial";

export class OrdenDeCarga {
    Id: number;
    CUITTercero: number;
    CUITCliente: number;
    CUITCorredor?: number;
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
    FechaVencimientoAmpliada: boolean;
    EdicionRechazada: boolean;
    constructor() {
    }


}

