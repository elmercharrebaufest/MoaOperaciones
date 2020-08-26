import { NuevoProduccion } from "./nuevoProduccion";
import { NuevoAcopio } from "./nuevoAcopio";
import { ContactoComercial } from "./contactoComercial";

export class InformeComercial {
    public Domicilio: string;
    public EmplRelDep: boolean;
    public EmplRelDepCant: string;
    public Rodados: number;
    public RodadosOtros: string;
    public Chacra: number;
    public ChacraOtros: string;
    public AntigActividad: string;
    public ActuacionProd: string;
    public ClienteAnt: string;
    public Comentarios: string;
    public Campania: string;
    public CampaniaId: number;
    public NuevosCampos: NuevoProduccion[];
    public NuevosAcopios: NuevoAcopio[]; 
    public direccion: string; 
    public codigoPostal: string; 
    public localidadId: number;
    public localidad: string;
    public ContactoComercial: ContactoComercial;

    constructor() {
        this.NuevosCampos = new Array<NuevoProduccion>();
        this.NuevosAcopios = new Array<NuevoAcopio>();
        this.ContactoComercial = new ContactoComercial();
    }
}
