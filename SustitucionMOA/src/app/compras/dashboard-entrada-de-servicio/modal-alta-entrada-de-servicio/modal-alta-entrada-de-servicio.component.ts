import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ModalService } from '../../../common/services/ModalService';
import { ComprasService } from '../../compras.service';

interface Item {
  id: string;
}

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

  @Input() itemSelected: any;
  @Input() elementSelected: any;
  @Input() itemIdSelected: string = '';
  @Output() closeModal = new EventEmitter<void>();

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

  constructor(private modalService: ModalService, protected service: ComprasService) { }

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
            PlannedPackage: '0003177389',
            PlannedLine: '0000000002'
          }
        ]
      }
    };

  
    this.service.postCreateAsync(this.entrySheetData).subscribe(
      (response) => {
        console.log('Respuesta del backend:', response);
        // Manejar la respuesta del backend
      },
      (error) => {
        console.error('Error en la solicitud:', error);
        this.errorResponseMessage = (error && error.error && error.error.message) || 'Error en la solicitud. Por favor, inténtelo de nuevo.';

        this.showError = true;
        // Manejar la respuesta del backend
      }
    );
    this.closeModal.emit();
  }  
                   
}
