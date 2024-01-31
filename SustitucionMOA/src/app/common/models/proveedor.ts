import { ProveedorHistorialAprobacion } from "./proveedor-historial-aprobaciones";

export class Proveedor {
    Id: number;
    CUIT: string;
    RazonSocial: string;
    CodigoProveedor: string;
    Mail: string;
    EstadoAprobacion: any;
    Observaciones: string;
    IdDataAgro: number | null;
    IdComercialDataAgro: number | null;
    EstadoAprobacionDescripcion: string;
    HistorialAprobaciones: ProveedorHistorialAprobacion[];
    Comercial: string;
    SISAEstadoCuit: string;
    EstadoSIPER: string;
    UltimaEdicion: string | null;
    FechaSolicitud: string | null;
    RazonSocialCorredor: string;
    IdTipoUsuario: number;
    IdTipoProveedor: number;
    IngresoAPlanta: boolean | null;
    AltaInterna: boolean | null;
    ContieneDocumentacionFisica: boolean | null;
    EsRevendedor: boolean;
    OrganizacionDeCompra: string | null;
}