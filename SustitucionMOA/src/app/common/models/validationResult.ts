export interface ValidationResult {
    IsValid: boolean;
    FileName?: string;
    Message?: string; 
    ValidataionType?: string; 
    Value?: string; 
    Input?: string;
    Certificaciones:{
        NroCertificacion: string;
        Saldo:number;
        Moneda:string;
        MontoFormateado:string;
        Archivo:any;
    }[];
}