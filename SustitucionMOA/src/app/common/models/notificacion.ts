import { Adjuntos } from "./adjuntos";
import { Rol } from "./rol";
import { TipoUsuario } from "./tipoUsuario";


export class Notificacion {
    public Id: number;
    public Nombre: string;
    public FechaInicio: Date;
    public FechaFin: Date;
    public HoraInicio: number;
    public Habilitada: boolean;
    public Borrada: boolean;
    public LinkAdjunto: string;
    public Mensaje: string;
    public FiltroRoles: Array<Rol>
    public Prioridad: number;
    public FechaCreacion: Date;
    public ArchivosAdjuntos: Array<Adjuntos>
    constructor() { 
        this.FiltroRoles = new Array<Rol>();
    }
}
