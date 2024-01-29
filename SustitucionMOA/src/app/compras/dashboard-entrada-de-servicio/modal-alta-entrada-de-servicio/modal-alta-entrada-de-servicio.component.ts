import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ComprasService } from '../../compras.service';
import { ConfirmationService } from 'primeng/api';

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

 


  @Input() showModal: boolean;
  @Input() itemSelected: any;
  @Input() elementSelected: any;
  @Input() itemIdSelected: string = '';
  @Output() closeModal = new EventEmitter<void>();

  @Output() enviarMensajeGrilla = new EventEmitter();


  

  entrySheetData = {
    "EntrySheetHeader": {
      "PaqueteNumero": "",
      "Descripcion": "",
      "OrdenCompraNumero": "",
      "OrdenCompraPosicionNumero": "",
      "DocumentoReferenciaNumero": "",
      "FechaDocumento": "",
      "FechaContabilizacion": "",
      "GrabarAceptada": ""
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

  constructor(protected service: ComprasService,
    private confirmationService: ConfirmationService
    ) { }

  ngOnInit() {
  }

  openModal() {
    this.step = 1;
  }

//   showDialog() {
//     this.visible = true;
// }

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
        GrabarAceptada: 'X'
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
        if (!response.data) {
          this.mensajeError = 'Error del servidor, vuelva a intentarlo más tarde.'
        }

        if(response.data.Type === 'I') {
          this.mensajeError = response.data.Message;
          this.confirmationService.confirm({
            message: this.mensajeError,
              accept: () => {
                this.enviarMensajeGrilla.emit();
                this.closeModal.emit();
              },
              reject: () => {
                this.closeModal.emit();
              }
            }
          );
        }

        if(response.data.Type === 'S') {
          this.mensajeError = response.data.Message;
          this.confirmationService.confirm({
            message: this.mensajeError,
              accept: () => {
                this.enviarMensajeGrilla.emit();
                this.closeModal.emit();
              },
              reject: () => {
                this.closeModal.emit();
              }
            }
          );
        }


        if(response.data.Type === 'E') {
          this.mensajeError = response.data.Message;
          this.confirmationService.confirm({
            message: this.mensajeError,
              accept: () => {
                this.closeModal.emit();
              },
              reject: () => {
                this.closeModal.emit();
              }
            }
          );
        }
      },
      (error) => {
        this.confirmationService.confirm({
          message: error.error.Message,
            accept: () => {
              this.closeModal.emit();
            },
            reject: () => {
              this.closeModal.emit();
            }
          }
        );
      }
    );
  } 
}
