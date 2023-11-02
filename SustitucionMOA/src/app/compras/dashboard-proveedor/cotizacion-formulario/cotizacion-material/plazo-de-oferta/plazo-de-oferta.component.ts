import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges, ViewChild } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbAlert } from '@ng-bootstrap/ng-bootstrap';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { ConfirmationService } from 'primeng/api';
import { PeticionDeOfertaDto } from '../../../../../modelos/peticion-de-oferta-model';
import { ComprasService } from '../../../../compras.service';
import { SecurityService } from '../../../../../common/services/SecurityService';
import { NavService } from '../../../../../common/services/NavService';
import { FloatMsgService } from '../../../../../common/services/FloatMsgService';
import { SessionDataService } from '../../../../../common/services/SessionDataService';
import { ModalService } from '../../../../../common/services/ModalService';
import { CotizacionPosicionDto } from '../../../../../modelos/cotizacionDto';

@Component({
  selector: 'app-plazo-de-oferta',
  templateUrl: './plazo-de-oferta.component.html',
  styleUrls: ['./plazo-de-oferta.component.css']
})
export class PlazoDeOfertaComponent implements OnInit, OnChanges {


  @Input()
  displayPlazo: boolean;
  @Input()
  public cantidadSolicitada: number;
  @Input()
  public cotizacionPosicion: CotizacionPosicionDto;
  @Output() cerrarPlazoEmitter = new EventEmitter();
  mostrarBotonAgregarSegundo = false;
  mostrarBotonOcultarSegundo = false;
  mostrarSegundaPosicion = false;
  mostrarTerceraPosicion = false;
  visualizarAlert: boolean;
  error: string;
  cantidad: number
  esCantidadCotizacion: boolean;

  constructor(protected service: ComprasService, protected navService: NavService, protected sessionDataService: SessionDataService,
    protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService,
    protected route: ActivatedRoute, protected router: Router, private confirmationService: ConfirmationService, private formBuilder: FormBuilder) {
  }
  ngOnInit(): void {

  }

  ngOnChanges(changes: SimpleChanges): void {
    if (this.cotizacionPosicion != null) {
      this.mostrarSegundaPosicion = this.cotizacionPosicion.SegundoPlazoDeOferta > 0 || this.cotizacionPosicion.SegundaCantidad > 0;
      this.mostrarBotonAgregarSegundo = !this.mostrarSegundaPosicion;
      this.mostrarTerceraPosicion = this.cotizacionPosicion.TercerPlazoDeOferta > 0 || this.cotizacionPosicion.TerceraCantidad > 0;
      this.mostrarBotonOcultarSegundo = this.mostrarTerceraPosicion;
      this.cantidad = this.cotizacionPosicion.Cantidad > 0 ?
        this.cotizacionPosicion.Cantidad : this.cantidadSolicitada
      this.esCantidadCotizacion = this.cotizacionPosicion.Cantidad > 0;
    }
  }

  onCerrarPlazo() {
    this.cerrarPlazoEmitter.next();
  }

  cancelarPlazo() {
    this.limpiarPrimeraPosicion();
    this.limpiarSegundaPosicion();
    this.limpiarTerceraPosicion();
    this.onCerrarPlazo();
  }

  aceptarPlazo() {
    if (!this.validarCantidadDeOferta()) {
      this.onCerrarPlazo();
    }
  }

  agregarSegundaPosicion() {
    this.visualizarAlert = false;
    if (this.cotizacionPosicion.PrimerPlazoDeOferta == 0 || this.cotizacionPosicion.PrimeraCantidad == 0) {
      this.error = "Debe completar plazo y cantidad para crear un nuevo registro";
      this.visualizarAlert = true;
    } else {
      this.mostrarBotonAgregarSegundo = false;
      this.mostrarBotonOcultarSegundo = true;
      this.mostrarSegundaPosicion = true;
    }
  }

  ocultarSegundaPosicion() {
    this.limpiarSegundaPosicion();
    this.mostrarSegundaPosicion = false;
    this.mostrarBotonAgregarSegundo = true;
  }

  agregarTerceraPosicion() {
    this.visualizarAlert = false;
    if (this.cotizacionPosicion.SegundoPlazoDeOferta == 0 || this.cotizacionPosicion.SegundaCantidad == 0) {
      this.error = "Debe completar plazo y cantidad para crear un nuevo registro";
      this.visualizarAlert = true;
    } else {
      this.mostrarTerceraPosicion = true;
    }
  }

  ocultarTerceraPosicion() {
    this.mostrarTerceraPosicion = false;
    this.limpiarTerceraPosicion();
  }

  limpiarPrimeraPosicion() {
    this.cotizacionPosicion.PrimerPlazoDeOferta = 0;
    this.cotizacionPosicion.PrimeraCantidad = 0;
  }

  limpiarSegundaPosicion() {
    this.cotizacionPosicion.SegundoPlazoDeOferta = 0;
    this.cotizacionPosicion.SegundaCantidad = 0;
  }

  limpiarTerceraPosicion() {
    this.cotizacionPosicion.TercerPlazoDeOferta = 0;
    this.cotizacionPosicion.TerceraCantidad = 0
  }

  validarCantidadDeOferta() {
    var noGuardar = false;
    this.visualizarAlert = false;

    const primeraCantidad = Number(this.cotizacionPosicion.PrimeraCantidad);
    const segundaCantidad = Number(this.cotizacionPosicion.SegundaCantidad);
    const terceraCantidad = Number(this.cotizacionPosicion.TerceraCantidad);

    const cantidadTotalIngresada = primeraCantidad + segundaCantidad + terceraCantidad;
    if (cantidadTotalIngresada > this.cantidad) {
      this.error = "La suma de las cantidades: " + this.formatearNumero(cantidadTotalIngresada)
        + " no debe superar la cantidad " + (this.esCantidadCotizacion ? "solicitada: " : "cotizada: ")
        + this.formatearNumero(this.cantidad);
      this.visualizarAlert = true;
      return noGuardar = true;
    }

    if (cantidadTotalIngresada != this.cantidad) {
      this.error = "La suma de las cantidades deben ser igual a la cantidad " + (this.esCantidadCotizacion ? "solicitada: " : "cotizada: ")
        + this.formatearNumero(this.cantidad);
      this.visualizarAlert = true;
      return noGuardar = true;
    }


    var validacion = this.validarCotizaciones();

    if (validacion != "") {
      this.error = validacion;
      this.visualizarAlert = true;
      return noGuardar = true;
    }

    return noGuardar;
  }

  formatearNumero(numero: number) {
    return numero.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,');
  }

  validarCotizaciones() {
    const plazosYOfertas = [
      { plazo: this.cotizacionPosicion.PrimerPlazoDeOferta, cantidad: Number(this.cotizacionPosicion.PrimeraCantidad) },
      { plazo: this.cotizacionPosicion.SegundoPlazoDeOferta, cantidad: Number(this.cotizacionPosicion.SegundaCantidad) },
      { plazo: this.cotizacionPosicion.TercerPlazoDeOferta, cantidad: Number(this.cotizacionPosicion.TerceraCantidad) },
    ];

    let etapaPlazo = 0;
    let etapaCantidad = 0;
    let cantidadAcumulada = 0;

    for (let i = 0; i < plazosYOfertas.length; i++) {
      const plazoOferta = plazosYOfertas[i];

      if (plazoOferta.plazo > 0 && etapaPlazo === i) {
      
          if (plazoOferta.cantidad > 0) {
            if (cantidadAcumulada + plazoOferta.cantidad > this.cantidad) {
              this.error = "La suma de las cantidades deben ser igual a la cantidad " + (this.esCantidadCotizacion ? "solicitada: " : "cotizada: ")
                + this.formatearNumero(this.cantidad);
            }
            cantidadAcumulada += plazoOferta.cantidad;
            etapaCantidad++;
          } else {
            return `Debe completar la cantidad en el ${i === 0 ? 'primer' : i === 1 ? 'segundo' : 'tercer'} plazo.`;
          }
          etapaPlazo++;
        
      } else if(cantidadAcumulada != this.cantidad){
        return `Debe completar el ${i === 0 ? 'primer' : i === 1 ? 'segundo' : 'tercer'} plazo antes de continuar.`;
      }

    }
    return "";
  }

  onEditarCelda(cotizacion: any, campo: string, valorInicial: any) {
    if (cotizacion[campo] === valorInicial) {
        cotizacion[campo] = ''; // Limpia el valor si es igual al valorInicial
    }
}

  onReestablecerValor(cotizacion: any, campo: string) {
      if (cotizacion[campo] === '' || cotizacion[campo] === null) {
          cotizacion[campo] = 0; // Restablece a cero si está en blanco
      }
  }

}