import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ComprasService } from '../../compras.service';
import { ConfirmationService } from 'primeng/api';
import { CalendarModule } from 'primeng/calendar';
declare var $: any;

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
  fechaDocumento: Date;
  fechaContabilizacion: Date;
  es: any;
  referencia: string;
  textoBreve: string;


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
        this.es = {
            firstDayOfWeek: 0,
            dayNames: ["Domingo", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado"],
            dayNamesShort: ["Dom", "Lun", "Mar", "Mié", "Jue", "Vie", "Sáb"],
            dayNamesMin: ["Do", "Lu", "Ma", "Mi", "Ju", "Vi", "Sá"],
            monthNames: ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"],
            monthNamesShort: ["Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic"],
            today: 'Hoy',
            clear: 'Limpiar',
            dateFormat: 'yyyy-mm-dd',
            weekHeader: 'Sem'
        };

  }

  ngAfterViewInit(): void{

  }

    ngAfterContentInit() {
        this.textoBreve = this.itemSelected[0].Descripcion !== undefined ? this.itemSelected[0].Descripcion : '' ;
    }

  openModal() {
    this.step = 1;
  }


    dateFormatter(date_Object: Date): string {
        if (date_Object !== undefined) {
            const year = date_Object.getFullYear();
            const month = (date_Object.getMonth() + 1 < 10 ? '0' : '') + (date_Object.getMonth() + 1);
            const day = (date_Object.getDate() < 10 ? '0' : '') + date_Object.getDate();

            const date_String: string = `${year}-${month}-${day}`;
            return date_String;
        }
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
    let fechaDocFormateada = "";
    let fechaConFormateada = "";

      if (this.fechaDocumento !== undefined) {
          fechaDocFormateada = this.dateFormatter(this.fechaDocumento);
      }
      if (this.fechaContabilizacion !== undefined) {
          fechaConFormateada = this.dateFormatter(this.fechaContabilizacion);
      }
      let ref = this.referencia !== undefined ? this.referencia : '';
      let txtBreve = this.textoBreve !== undefined ? this.textoBreve : '';


    this.entrySheetData = {
      EntrySheetHeader: {
        PaqueteNumero: '0000000001',
        Descripcion: this.itemSelected[0].Descripcion,
        OrdenCompraNumero: this.elementSelected.NumeroOrdenDeCompra,
        OrdenCompraPosicionNumero: this.itemSelected[0].NroPosicion.toString(),
        DocumentoReferenciaNumero: ref,
        FechaDocumento: fechaDocFormateada,
        FechaContabilizacion: fechaConFormateada,
        GrabarAceptada: 'X'
      },
      EntrySheetServices: {
        Items: [
          {
            PackageNumber: '0000000002',
            LineNumber: '0000000002',
            ExternalLine: '0000000010',
            Service: this.itemSelected[0].ServicioNumero.toString(),
            Quantity: this.cantidad !== undefined ? this.cantidad : this.itemSelected[0].Cantidad,
            GrossPrice: this.itemSelected[0].PrecioBruto !== undefined ? this.itemSelected[0].PrecioBruto.toString() : '',
            ShortText: txtBreve,
            PlannedPackage: this.itemSelected[0].Id,
            PlannedLine: this.itemSelected[0].LINE_NO
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

  cerrarModal() {
    this.closeModal.emit();
  }
}
