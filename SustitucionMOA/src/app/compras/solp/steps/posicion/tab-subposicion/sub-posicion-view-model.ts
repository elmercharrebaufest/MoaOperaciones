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
      this.cuentaTd = 0;
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
   cuentaTd: number;
   unidadMedida: string;
   precioBruto: number;
   tipoImputacion: any = {};
   unidadSeleccionada : any = {};
   monedaSeleccionada: any;
   valorNeto: number;

   public calcularValorNeto(): void
   {
      this.valorNeto = (this.precioBruto || 0) * (this.cuentaTd || 0);
      this.valorNeto = parseFloat(this.valorNeto.toFixed(2));
   }
}