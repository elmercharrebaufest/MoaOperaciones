export class EstadoOrdenResiduos {
    public Id: number;
    public Nombre: string;
    public NombreExterno: string;
    public Semaforo: string;
}

export enum EstadoOrdenResiduosEnum {
    OrdenGenerada = 1,
    Pendiente = 2,
    OrdenVencida = 3,
    OrdenEntregada = 4,
    Anulada = 5,
    // EdicionSolicitada = 6,
    // EdicionRechazada = 7,
    // AnulacionSolicitada = 8
    Ingresada = 9,
    Retirada = 10,
    Rechazada = 11
}