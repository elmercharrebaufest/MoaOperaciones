export class Balanza {
    codigo: number;
    descripcion: string;
    automatico: boolean;
    toleria: number;
    centroEmisor: string;
    tolerX: number;
    tipoId: string;
    pesoMaximo: number;
    codigoSAP: number;
    codigoCabezalId: string;
    itcId: string;
    nroPuestoId: string;
    tipoAccesoId: string;
    commodityId: string;
    exportadorId: string;

}

export class Commodity {
    materialSAP: string;
    descripcion: string;
    commodityId: string;
    almacenOrigen: string;

}

export class Exportador {
    almacenSAP: string;
    descripcion: string;
    exportadorId: string;
}

export class BalanzaBusqueda {
    descripcion: string;
    tipoId: string;
    codigoSAP: number;
    codigoCabezalId: string;
}

export class BalanzaAplicar {
    codigo: number;
}