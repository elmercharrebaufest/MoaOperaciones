import { Etapa, DatosAdicionales } from "./estado-etapas.model";

export interface TrackingResponse {
    resultado: boolean;
    mensaje: string;
    data?: {
        workflow: string;
        ctg: string;
        fechaHoraIngreso: Date;
        titularCartaPorte: string;
        remitenteComercial: string;
        remitenteComercialVtaPrim: string;
        entregador: string;
        transportista: string;
        material: string;
        rechazado: boolean;
        camion: {
            patente: string;
            patenteAcoplado: string;
        };
        chofer: {
            cuil: string;
            tipoDocumento: string;
            numeroDocumento: string;
            extranjero: string;
            nombreApellido: string;
        };
        datosAdicionales: DatosAdicionales;
        etapas: Etapa[];
    };
}