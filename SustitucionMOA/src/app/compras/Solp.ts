import { Time } from "@angular/common";

export class Solp {
    //paso 1
    public nombreDeObra: string;
    public fiscalContrato: string;
    public telefono: string;
    public mail: string;
    public fechaDeEntregaDeOfertasFecha: Date;
    public fechaDeEntregaDeOfertasHora: Time;


    //paso 2
    public visitaDeObra: boolean;
    public supervisorSector: string;
    public visitaDeObraFecha: Date;
    public visitaDeObraHora: Time;
    public supervisorTrabajo: string;
    public obradores: boolean;
    public descripcionTecnica: boolean;
    public modoElevacion: boolean;
    public entregaDocumentacion: boolean;
    public tecnicoSeguridad: boolean;
    public fechaLimiteFecha: Date;
    public fechaLimiteHora: Time;

}

