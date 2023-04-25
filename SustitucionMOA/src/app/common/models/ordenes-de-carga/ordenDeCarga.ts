import { EstadoOrdenDeCarga } from "./estadoOrdenDeCarga";
import { ContratoOrdenFas } from "./obtenerContratosDisponiblesResponse";
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
    ContratoSeleccionado: ContratoOrdenFas | undefined;
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
    CUITDestinatario?: string;
    CUITDestino?: string;
    RazonSocialDestinatario?: string;
    RazonSocialDestino?: string;
    Reventa: boolean;
    CUITIntermediarioFlete?: string;
    PlantaCodigo: string;
    DomicilioTipo: string;
    DomicilioOrden: number;
    DomicilioDescr: string;
    constructor() {
    }
}
export type CuitValidaExistencia = keyof Pick<OrdenDeCarga, "CUITDestinatario" | "CUITDestino">;
export type CuitValidaSISA = keyof Pick<OrdenDeCarga, "CUITDestinatario" | "CUITDestino" | "CUITCorredor" | "CUITCliente">;
export type CuitValidaRUCA = keyof Pick<OrdenDeCarga, "CUITDestinatario" | "CUITDestino" |  "CUITCliente">;

