export class EcheqFilter {
    public contrato: string;
    public tipoContrato: string;

    public periodo: string;
    public fechaInicio: string;
    public fechaFin: string;

    constructor() {
        this.tipoContrato = "Todos";
    }
}