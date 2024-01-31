export class MisEcheqFilter {
    public RazonSocial: string;
    public Mail: string; 
    public CodigoProveedor: string;
    public Contrato: string;
    public Liquidacion: string;
    public periodo: string;
    public fechaInicio: string;
    public fechaFin: string;
    public LiquidacionMarcada: string = "null";
    public EcheqGenerados: string = "null";

    constructor() {
    }
}