export interface EntrySheet {

    Header: Header;
    Service: Service;

}

export interface Header {
    SolPedNumber: string[];        
    MontoTotalACertificar: string;
    PaqueteNumero: string;
    Descripcion: string;
    OrdenCompraNumero: string;
    OrdenCompraPosicionNumero: string;
    DocumentoReferenciaNumero: string;
    FechaDocumento: string;
    FechaContabilizacion: string;
    GrabarAceptada: string;
    Proveedor: string;
}

export interface Service {
    items: item[]; 
}

export interface item {

     PackageNumber: string;  // se puede omitir del front
     LineNumber: string; // se puede omitir del front
     OutlineIndicator:  string; // se puede omitir del front
     SubPackageNumber:  string; // se puede omitir del front
     ExternalLineNumber:  string;
     Service:  string;
     Quantity:  string;
     ItemQuantity:  string;
     UM: string;
     ItemGrossPrice:  string;
     GrossPrice:number;
     Percentage:string;
     CertificationAmount:string;
     ShortText:  string;
     PlannedPackage:  string;
     PlannedLine:string;
     Descripcion:string;

}
