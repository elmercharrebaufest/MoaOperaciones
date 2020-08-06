import { NuevoProduccion } from "./nuevoProduccion";
import { NuevoAcopio } from "./nuevoAcopio";

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

  public Producciones: NuevoProduccion[];
  public Acopios: NuevoAcopio[];

  constructor() {}
}
