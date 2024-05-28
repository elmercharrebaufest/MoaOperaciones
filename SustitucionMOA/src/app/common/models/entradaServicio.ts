export class EntradaServicio {
    public PaqueteNumero: string;
    public Descripcion: string;
    public OrdenCompraNumero: number;
    public OrdenCompraPosicionNumero: string;
    public DocumentoReferenciaNumero: string;
    public FechaDocumento: Date;
    public FechaContabilizacion: Date;
    public GrabarAceptada: boolean;
    public EntrySheetHeader: {
        PaqueteNumero: string;
        Descripcion: string;
        OrdenCompraNumero: string;
        OrdenCompraPosicionNumero: string;
        DocumentoReferenciaNumero: string;
        FechaDocumento: string;
        FechaContabilizacion: string;
        GrabarAceptada: boolean;
    };
    public EntrySheetServices: {
        Items: Array<{
            PackageNumber: string;
            LineNumber: string;
            OutlineIndicator: string;
            SubPackageNumber: string;
            Quantity: string;
            ExternalLine?: string;
            Service?: string;
            GrossPrice?: number;
            ShortText?: string;
            PlannedPackage?: string;
            PlannedLine?: string;
        }>;
    };
}