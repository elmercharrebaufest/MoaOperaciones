import { EstadoOrdenDeCarga } from "./estadoOrdenDeCarga";
import { ContratoOrdenFas, TipoContrato } from './obtenerContratosDisponiblesResponse';
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
    CodigoCorredor: string;
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
    ValidaSisaRuca: boolean;
    CUITDestinatario?: string;
    CUITDestino?: string;
    RazonSocialDestinatario?: string;
    RazonSocialDestino?: string;
    Reventa: boolean;
    CUITIntermediarioFlete?: string;
    RazonSocialIntermediarioFlete?: string;
    PlantaCodigo: string;
    DomicilioTipo: string;
    DomicilioOrden: number;
    DomicilioDescr: string;
    NumeroFactura: string;
    NumeroFacturaSeleccionada: string;
    TipoContrato: TipoContrato;
    Escalable: boolean;
    NecesitaVerificarCuitsTerceros: boolean;

    constructor() {
    }
}
export type CuitValidaExistencia = keyof Pick<OrdenDeCarga, "CUITDestinatario" | "CUITDestino">;
export type CuitValidaSISA = keyof Pick<OrdenDeCarga, "CUITDestinatario" | "CUITDestino" | "CUITCorredor" | "CUITCliente">;
export type CuitValidaRUCA = keyof Pick<OrdenDeCarga, "CUITDestinatario" | "CUITDestino" | "CUITCliente">;

export const KILOS_DISPONIBLES_APROBADO = 15000
export const SIN_KILOS_DISPONIBLES = 0

export const VOLVER_A_DETALLE_REPORTE = 'volverADetalleReporteContrato'


