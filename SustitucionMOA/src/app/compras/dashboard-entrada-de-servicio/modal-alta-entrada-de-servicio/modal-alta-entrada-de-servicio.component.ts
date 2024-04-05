import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ComprasService } from '../../compras.service';
import { ConfirmationService } from 'primeng/api';
import { CalendarModule } from 'primeng/calendar';
import { forEach } from '@angular/router/src/utils/collection';
import { FormsModule } from '@angular/forms';
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
    //MMSN-648
    arrCantidad: any[] = new Array();
    entrySheetObjects: any = [];

    es: any;
    referencia: string = '';
    textoBreve: string = '';
    //Provisional - hasta definición de funcionalidad de aprobador.
    //aprobador: string = '';

    @Input() showModal: boolean;
    @Input() itemSelected: any;
    @Input() elementSelected: any;
    @Input() itemIdSelected: string = '';
    @Output() closeModal = new EventEmitter<void>();
    @Output() closeDialog = new EventEmitter<void>();

    @Output() enviarMensajeGrilla = new EventEmitter();

    fechaDocMin: Date;
    fechaDocMax: Date;
    fechaContabilizacionMin: Date;
    fechaContabilizacionMax: Date;

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
                    "ExternalLineNumber": "",
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

    totalMontoCertificar!: number;

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

        this.fechaContabilizacion = new Date();
        this.setRangoFechaDocumento();
        this.setRangoFechaContabilizacion();
        this.calcularTotalMontoCertificar();
    }

    ngAfterViewInit(): void {

    }

    ngAfterContentInit() {
        this.textoBreve = this.itemSelected[0].Descripcion !== undefined ? this.itemSelected[0].Descripcion : '';
    }

    openModal() {
        this.step = 1;
    }

    setRangoFechaDocumento() {
        // Fecha máxima: Fecha actual
        this.fechaDocMax = new Date();
        // Fecha mínima: Fecha Actual 5 años hacia atrás
        this.fechaDocMin = new Date(new Date().setFullYear(new Date().getFullYear() - 5));
    }

    setRangoFechaContabilizacion() {
        // Fecha máxima: Fecha actual
        this.fechaContabilizacionMax = new Date();
        // Fecha mínima: Primero del mes corriente
        this.fechaContabilizacionMin = new Date(new Date().getFullYear(), new Date().getMonth(), 1);
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

    //Completa con '0' el valor de this.itemSelected.NumeroLinea - necesario para el servicio
    zeroPad(num, places) {
        var zero = places - num.toString().length + 1;
        return Array(+(zero > 0 && zero)).join("0") + num;
    }

    groupByNroPosicion(data) {

        const grouped = {};

        data.forEach(item => {
            const nroPosicion = item.NroPosicion.toString();

            if (!grouped[nroPosicion]) {
                grouped[nroPosicion] = [];
            }

            grouped[nroPosicion].push(item);
        });

        return grouped;
    }

    certificarPosicion() {

        const groupedData = this.groupByNroPosicion(this.itemSelected);

        this.buildEntrySheet(groupedData);

        this.service.postCreateAsync(this.entrySheetObjects).subscribe(
            (response) => {

                let resultMsj: string[] = [];

                response.data.forEach(element => {
                    if (!element) {
                        this.mensajeError = 'Error del servidor, vuelva a intentarlo más tarde.'
                    }

                    switch (element.Type) {
                        case "I": {
                            resultMsj.push(element.Message);
                            break;
                        }
                        case "S": {
                            resultMsj.push(element.Message);
                            break;
                        }
                        case "E": {
                            let msjError = element.Message.startsWith("Sólo es posible contabilizar en ") ||
                                element.Message.startsWith("Contabilice en ") ?
                                "El período se encuentra cerrado, por favor contabilice en el periodo actual." : element.Message
                            resultMsj.push(msjError);
                            break;
                        }
                        default: {
                            resultMsj;
                            break;
                        }
                    }
                });

                for (let msj of resultMsj) {
                    this.mensajeError += "<li>" + msj + "</li>";
                }

                this.confirmationService.confirm({
                    message: "<ul>" + this.mensajeError + "</ul>",
                    accept: () => {
                        this.enviarMensajeGrilla.emit();
                        this.closeModal.emit();
                    },
                    reject: () => {
                        this.closeModal.emit();
                    }
                }
                );
            },
            (error) => {
                this.confirmationService.confirm({
                    message: error.error.Message,
                    accept: () => {
                        this.closeDialog.emit();
                    },
                    reject: () => {
                        this.closeDialog.emit();
                    }
                }
                );
            }
        );

    }

    buildEntrySheet(groupedData: any) {

        let fechaDocFormateada = "";
        let fechaConFormateada = "";
        let PONumber = this.elementSelected !== undefined ? this.elementSelected.NumeroOrdenDeCompra : '';
        //Array Cantidad
        this.itemSelected.forEach((el) => {
            this.arrCantidad.push(el.CantidadACertificar !== undefined ? el.CantidadACertificar.toString() : '');
        })

        if (this.fechaDocumento !== undefined) {
            fechaDocFormateada = this.dateFormatter(this.fechaDocumento);
        }
        if (this.fechaContabilizacion !== undefined) {
            fechaConFormateada = this.dateFormatter(this.fechaContabilizacion);
        }

        let ref = this.referencia !== undefined ? this.referencia : '';
        //MMSN-678 - Modificar descripción por short_text en cabecera
        let txtBreve = this.textoBreve !== undefined ? this.textoBreve : '';

        for (const key in groupedData) {
            const group = groupedData[key];

            const itemSelected = group[0];

            const entrySheetHeader = {
                PaqueteNumero: key,
                Descripcion: txtBreve,
                OrdenCompraNumero: PONumber,
                OrdenCompraPosicionNumero: itemSelected.NroPosicion.toString(),
                DocumentoReferenciaNumero: ref,
                FechaDocumento: fechaDocFormateada,
                FechaContabilizacion: fechaConFormateada,
                GrabarAceptada: 'X'
            };

            const entrySheetServiceItems = group.map((item, index) => ({
                PackageNumber: '0000000002',
                LineNumber: '0000000002',
                ExternalLineNumber: this.zeroPad(item.NumeroLinea, 10),
                Service: item.ServicioNumero.toString(),
                Quantity: item.CantidadACertificar,
                GrossPrice: item.PrecioBruto !== undefined ? item.PrecioBruto.toString() : '',
                ShortText: txtBreve,
                PlannedPackage: item.Id,
                PlannedLine: item.LINE_NO
            }));

            const entrySheetServices = {
                Items: entrySheetServiceItems
            };

            this.entrySheetObjects.push({
                EntrySheetHeader: entrySheetHeader,
                EntrySheetServices: entrySheetServices
            });
        }
    }

    cerrarModal() {
        this.closeModal.emit();
    }

    validarReferenciaRemito(event) {
        let element = document.getElementById("ref");

        if (element.classList.contains('ng-dirty') && element.classList.contains('ng-invalid')) {
            element.classList.add('error');
        } else {
            element.classList.remove('error');
        }
    }

    calcularTotalMontoCertificar() {
        let total = 0;
        for (let item of this.itemSelected) {
            total += item.MontoACertificar;
        }

        this.totalMontoCertificar = total;
    }

    /**
     * Calcula el monto certificado anteriormente
     * a la certificacion actual.
     */
    calcularMontoAnterior(item: any): number {
        item.MontoAnterior = (item.Porcentaje * item.Importe) / 100;
        return item.MontoAnterior;
    }
}
