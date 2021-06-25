import * as uuid from 'uuid';
export class SubPosicionViewModel {

   constructor(subposicion: number) {
      this.id = uuid.v4();
      this.seleccionado = false;
      this.eliminar = false;
      this.subPosicion = subposicion;
      this.codigoServicio = "";
      this.tareaSubcontratar = "";
      this.cuentaMayor = "";
      this.cuentaTd = "";
      this.unidadMedida = "";
      this.precioBruto = 0;
      this.tipoImputacion = "";
   }


   id: string;
   seleccionado: boolean;
   eliminar: boolean;
   subPosicion: number;
   codigoServicio: string;
   tareaSubcontratar: string;
   cuentaMayor: string;
   cuentaTd: string;
   unidadMedida: string;
   precioBruto: number;
   tipoImputacion: string;
   unidadSeleccionada : any ={};
}