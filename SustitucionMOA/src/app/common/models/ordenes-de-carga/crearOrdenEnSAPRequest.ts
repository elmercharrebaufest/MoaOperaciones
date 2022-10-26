export interface CrearOrdenEnSAPRequest {
    IdOrdenDeCarga: number;
    ClienteCodigo: string;
    ContratoSAP: string;
    CorredorCodigo: string;
    Cantidad: number;
    MaterialCodigoSAP: string;
    NumeroPedidoIngresado: string;
    // ValidarKg: string;
    MailUsuarioSAP: string;
}

export interface CrearOrdenEnSAPResponse {
    ResultCreation: boolean;
    Error: string;
}