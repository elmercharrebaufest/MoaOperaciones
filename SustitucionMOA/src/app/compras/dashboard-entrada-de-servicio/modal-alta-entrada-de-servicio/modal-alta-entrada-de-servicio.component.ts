import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { ComprasService } from '../../compras.service';
import { ListadoDashboardCertificacionDeServiciosComponent } from '../listado-dashboard-certificacion-de-servicios/listado-dashboard-certificacion-de-servicios.component';

@Component({
  selector: 'app-modal-alta-entrada-de-servicio',
  templateUrl: './modal-alta-entrada-de-servicio.component.html',
  styleUrls: ['./modal-alta-entrada-de-servicio.component.css']
})

export class ModalAltaEntradaDeServicioComponent implements OnInit {

  step: number = 1;
  showAllTables: boolean = false;
  data: any;
  errorResponseMessage: string = "";
  showError: boolean = false;
  cantidad: number = 0;
  mensajeError: string = "";

  @Input() itemSelected: any;
  @Input() elementSelected: any;
  @Input() itemIdSelected: string = '';
  @Output() closeModal = new EventEmitter<void>();

  @Output() enviarMensajeError = new EventEmitter();

  entrySheetData = {
    "EntrySheetHeader": {
      "PaqueteNumero": "",
      "Descripcion": "",
      "OrdenCompraNumero": "",
      "OrdenCompraPosicionNumero": "",
      "DocumentoReferenciaNumero": "",
      "FechaDocumento": "",
      "FechaContabilizacion": "",
      "GrabarAceptada": false
    },
    "EntrySheetServices": {
      "Items": [
        {
          "PackageNumber": "",
          "LineNumber": "",
          "OutlineIndicator": "",
          "SubPackageNumber": "",
          "Quantity": ""
        },
        {
          "PackageNumber": "",
          "LineNumber": "",
          "ExternalLine": "",
          "Service": "",
          "Quantity": "",
          "GrossPrice": "",
          "ShortText": "",
          "PlannedPackage": "",
          "PlannedLine": ""
        }
      ]
    }
  };

  constructor(protected service: ComprasService
    ) { }

  ngOnInit() {
  }

  openModal() {
    this.step = 1;
  }

  siguientePaso(cantidad) {
    this.cantidad = cantidad;

    if (this.step < this.itemSelected.length) {
      this.step++;
    } else {
      this.showAllTables = true;
    }
  }

  esFilaPar(index: number): boolean {
    return index % 2 === 0;
  }

  refresh() {
    //Agregar lógica para refrescar Cantidades
  }

  certificarPosicion() {
    const fechaActual = new Date();
    const fechaFormateada = fechaActual.toISOString().split('T')[0];

    const primeraPosicion = this.elementSelected.Posiciones[0];
    const primerItem = primeraPosicion.Items[0];

    this.entrySheetData = {
      EntrySheetHeader: {
        PaqueteNumero: '0000000001',
        Descripcion: primerItem.Descripcion,
        OrdenCompraNumero: this.elementSelected.NumeroOrdenDeCompra,
        OrdenCompraPosicionNumero: primeraPosicion.NumeroPosicion.toString(),
        DocumentoReferenciaNumero: '',
        FechaDocumento: fechaFormateada,
        FechaContabilizacion: fechaFormateada,
        GrabarAceptada: true
      },
      EntrySheetServices: {
        Items: [
          {
            PackageNumber: '0000000001',
            LineNumber: '0000000001',
            OutlineIndicator: 'X',
            SubPackageNumber: '0000000002',
            Quantity: this.cantidad !== undefined ? this.cantidad : primerItem.Cantidad,
          },
          {
            PackageNumber: '0000000002',
            LineNumber: '0000000002',
            ExternalLine: '0000000010',
            Service: primerItem.ServicioNumero.toString(),
            Quantity: this.cantidad !== undefined ? this.cantidad : primerItem.Cantidad,
            GrossPrice: primerItem.PrecioBruto,
            ShortText: primerItem.Descripcion,
            PlannedPackage: primerItem.Id,
            PlannedLine: primerItem.LINE_NO
          }
        ]
      }
    };

  
    this.service.postCreateAsync(this.entrySheetData).subscribe(
      (response) => {
        if(response.data.Type === 'S'){
          this.mensajeError = (response && response.data.Message);
          this.enviarMensajeError.emit(response.data.Message);
        }
        if(response.data.Type === 'E'){
          this.mensajeError = (response && response.data.Message) || 'Error en la solicitud. Por favor, inténtelo de nuevo.';
          this.enviarMensajeError.emit(response.data.Message);
        }
        this.closeModal.emit();
      },
      (error) => {
        this.mensajeError = (error && error.error && error.error.message) || 'Error en la solicitud. Por favor, inténtelo de nuevo.';
        this.enviarMensajeError.emit(this.mensajeError);
        this.closeModal.emit();
      }
    );
  } 
}
