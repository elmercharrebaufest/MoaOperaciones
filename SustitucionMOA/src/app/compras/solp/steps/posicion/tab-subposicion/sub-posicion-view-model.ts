import * as uuid from 'uuid';
export class SubPosicionViewModel {

   constructor(subposicion: number) {
      this.id = uuid.v4();
      this.seleccionado = false;
      this.eliminar = false;
      this.subPosicion = subposicion;
      this.codigoServicio = {};
      this.tareaSubcontratar = "";
      this.tareaSubcontratarObj = {};
      this.cuentaMayor = {};
      this.cuentaTd = "";
      this.unidadMedida = "";
      this.precioBruto = 0;
      this.monedaSeleccionada = this.monedaSeleccionada;
      this.valorNeto = 0;
   }

   id: string;
   seleccionado: boolean;
   eliminar: boolean;
   subPosicion: number;
   codigoServicio: any;
   tareaSubcontratar: string;
   tareaSubcontratarObj: any;
   cuentaMayor: any;
   cuentaTd: any;
   unidadMedida: string;
   precioBruto: number;
   tipoImputacion: any = {};
   unidadSeleccionada : any = {};
   monedaSeleccionada: any;
   valorNeto: any;

   public calcularValorNeto(): void
   {
      this.valorNeto = (this.precioBruto || 0) * (parseInt(this.cuentaTd) || 0); 
   }
}