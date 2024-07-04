import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { ComprasService } from '../../compras.service';
import { ConfirmationService, Message } from 'primeng/api';
import { CalendarModule } from 'primeng/calendar';
import { forEach } from '@angular/router/src/utils/collection';
import { FormsModule } from '@angular/forms';
import { UsuarioService } from '../../../usuario/usuario.service';
import { Calendar } from 'primeng/calendar';
import { reference } from '@angular/core/src/render3';
import { SessionDataService } from '../../../common/services/SessionDataService';
import { FloatMsgService } from '../../../common/services/FloatMsgService';

declare var $: any;

type Column = {
    name: string;
    visible: boolean;
};

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
    solPed: string = '';

    es: any;
    referencia: string = '';
    textoBreve: string = '';
    //Provisional - hasta definición de funcionalidad de aprobador.
    //aprobador: string = '';

    @Input() showModal: boolean;
    @Input() itemSelected: any;
    @Input() elementSelected: any;
    @Input() itemIdSelected: string = '';
    @Input() posicionSelected: any;
    @Output() closeModal = new EventEmitter<void>();
    @Output() closeDialog = new EventEmitter<void>();

    @Output() enviarMensajeGrilla = new EventEmitter();

    itemsAgrupadosPorPosicion: any[] = [];

    fechaDocMin: Date;
    fechaDocMax: Date;
    fechaContabilizacionMin: Date;
    fechaContabilizacionMax: Date;
    colConfigName: string = 'columnasAltaCertificaciones';
    colspanMonto: number = 10;
    colConfig = [];
    monthNavStatus: boolean = false;
    documentDateMsg: Message[] = [];
    certificarState: boolean = false;

    entrySheetData = {
        "EntrySheetHeader": {
            "SolPedNumber": "",
            "MontoTotalACertificar": "",
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
                    "ItemQuantity": "",
                    "UM": "",
                    "ItemGrossPrice": "",
                    "GrossPrice": "",
                    "Percentage": "",
                    "CertificationAmount": "",
                    "ShortText": "",
                    "PlannedPackage": "",
                    "PlannedLine": ""
                }
            ]
        }
    };

    totalMontoCertificar!: number;

    constructor(protected service: ComprasService,
        private confirmationService: ConfirmationService,
        protected sessionDataService: SessionDataService,
        protected floatMsgService: FloatMsgService
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
        //this.fechaDocumento = new Date();
        this.setRangoFechaDocumento();
        this.setRangoFechaContabilizacion();
        this.calcularTotalMontoCertificar();
        this.itemSelected = this.orderBy(this.itemSelected, 'NroPosicion');
        this.colConfig = this.getColumnConfig();
    }

    ngAfterViewInit(): void {
    }

    ngAfterContentInit() {
        this.entrySheetObjects = [];
        this.agruparItemPorPosicion(this.itemSelected);
        this.applyColumnConfig(this.colConfig);
    }

    agruparItemPorPosicion(items) {
        this.itemsAgrupadosPorPosicion = items.reduce((prev, { NroPosicion, NroSolP, ...Items }) => {
            const id = prev.findIndex((item) => item.NroPosicion === NroPosicion);
            if (id >= 0) {
                prev[id].MontoTotalACertificar = prev[id].MontoTotalACertificar + (Items.CantidadACertificar * Items.Importe);
                prev[id].Descripcion.trim();
                prev[id].Items.push(Items);
            } else {
                prev.push({ NroPosicion, Descripcion: Items.Descripcion.trim(), Items: [Items], MontoTotalACertificar: (Items.CantidadACertificar * Items.Importe), NroSolP })
            }
            return prev;
        }, []);
    }

    openModal() {
        this.step = 1;
    }

    orderBy(array: any[], field: string): any[] {
        return array.sort((a, b) => a[field] - b[field]);
    }


    calculateGeneralTotalAmount(): string {
        let montoTotalGeneral = 0;
        let moneda: string = ''
        moneda = this.itemsAgrupadosPorPosicion[0].Items[0].Moneda;

        this.itemsAgrupadosPorPosicion.forEach(position => {
            montoTotalGeneral += position.MontoTotalACertificar;
        });

        if (moneda === 'ARP') {
            return `$ ${montoTotalGeneral.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`;
        } else {
            return `${montoTotalGeneral.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`;
        }

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
        if (sessionStorage.permisos !== undefined && sessionStorage.permisos.includes('ADMIN CONTABILIZACION MES ANTERIOR')) {
            this.fechaContabilizacionMin = new Date(new Date().getFullYear(), new Date().getMonth() - 1, 1);
            this.monthNavStatus = true;
        }
        else {
            this.fechaContabilizacionMin = new Date(new Date().getFullYear(), new Date().getMonth(), 1);
        }
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

    cerrarMensajes(msjTypes: string[]) {
        // Si alguna de los items se certificaron exitosamente actualizar tabla de ordenes
        if (msjTypes.includes('I') || msjTypes.includes('S')) {
            this.enviarMensajeGrilla.emit();
        }
        // Si no se recibión un error cerrar Modal de Certificaciones
        if (!msjTypes.includes('E')) {
            this.closeModal.emit();
        }
        // Si alguna certificación falló, cerrar mensaje pero mantener Modal de Certificaciones abierto
        else {
            this.closeDialog.emit();
        }
    }

    async certificarPosicion() {

        if (this.validateValues() === true) {
            
            this.certificarState = true;
            this.buildEntrySheet();

            let items = this.itemSelected;

            items = items.map(element => {
                element.EntradasServicio = [];
                return element;
            });

            this.service.postCreateAsync(this.entrySheetObjects, items).subscribe(
                (response) => {
                    this.mensajeError = '';
                    let resultMsj: string[] = [];
                    let msjTypes: string[] = [];

                    response.data.forEach(element => {
                        if (!element) {
                            this.mensajeError = 'Error del servidor, vuelva a intentarlo más tarde.'
                        }

                        if (!element.Message) {
                            element.Message = "Ha ocurrido un error por favor inténtelo nuevamente más tarde."
                        }

                        let msj = element.Message.startsWith("Sólo es posible contabilizar en ") ||
                            element.Message.startsWith("Contabilice en ") ?
                            "El período se encuentra cerrado, por favor contabilice en el periodo actual." : element.Message;

                        resultMsj.push("<li>" + msj + "</li>");

                        if (!msjTypes.includes(element.Type)) {
                            msjTypes.push(element.Type);
                        }
                    });

                    this.mensajeError = resultMsj.join("");
                    
                    this.confirmationService.confirm({
                        message: "<ul>" + this.mensajeError + "</ul>",
                        accept: () => this.cerrarMensajes(msjTypes),
                        rejectVisible: false
                    });
                    this.certificarState = false;
                },
                (error) => {
                    this.confirmationService.confirm({
                        message: error.error.Message,
                        accept: () => {
                            this.closeDialog.emit();
                        },
                        rejectVisible: false
                    }
                    );
                    this.certificarState = false;
                }
            );

        }
      
    }

    tituloArchivoPDF = "Reporte";
    BuildReport(){

        this.service.buildReportES(this.itemSelected).subscribe(
            (result) => {
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.floatMsgService.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.floatMsgService.setInfoMsg(result.info);
                } else {
                    var byteArray = new Uint8Array(result.FileContents);
                    var blob = new Blob([byteArray], { type: 'application/pdf' });
                    if (window.navigator.msSaveOrOpenBlob) {
                        // IE11
                        window.navigator.msSaveOrOpenBlob(blob, result.FileDownloadName + ".pdf");
                    } else {
                        var url = window.URL.createObjectURL(blob);
                        var link = document.createElement("a");
                        document.body.appendChild(link);
                        link.href = url;
                        link.download = this.tituloArchivoPDF + new Date() + ".pdf"
                        link.click();
                        setTimeout(function () { window.URL.revokeObjectURL(url); }, 0);
                        return false;
                    }
                }
            },
            error => {
                this.floatMsgService.setErrorMsg(error.message);
            });
    }

    buildEntrySheet() {
        let fechaDocFormateada = "";
        let fechaConFormateada = "";
        let PONumber = this.elementSelected !== undefined ? this.elementSelected.NumeroOrdenDeCompra : '';
        this.solPed = this.posicionSelected !== undefined ? this.posicionSelected.NumeroSolp : '';
        this.entrySheetObjects = [];

        if (this.fechaDocumento !== undefined) {
            fechaDocFormateada = this.dateFormatter(this.fechaDocumento);
        }

        if (this.fechaContabilizacion !== undefined) {
            fechaConFormateada = this.dateFormatter(this.fechaContabilizacion);
        }

        let ref = this.referencia !== undefined ? this.referencia : '';

        this.itemsAgrupadosPorPosicion.forEach(position => {

            let solpedNumbers = this.elementSelected.Posiciones.filter(posicion => posicion.isSelected).map(posicion => posicion.NumeroSolp);

            const entrySheetHeader = {
                SolPedNumber: solpedNumbers.length > 0 ? solpedNumbers : [this.solPed],
                MontoTotalACertificar: this.round(this.totalMontoCertificar, 2).toString(),
                PaqueteNumero: position.NroPosicion.toString(),
                Descripcion: position.Descripcion.trim(),
                OrdenCompraNumero: PONumber,
                OrdenCompraPosicionNumero: position.NroPosicion.toString(),
                DocumentoReferenciaNumero: ref.trim(),
                FechaDocumento: fechaDocFormateada,
                FechaContabilizacion: fechaConFormateada,
                GrabarAceptada: 'X',
                Proveedor: this.elementSelected.Proveedor
            };

            const entrySheetServiceItems = position.Items.map((item, index) => ({
                PackageNumber: '0000000002',
                LineNumber: '0000000002',
                ExternalLineNumber: this.zeroPad(item.NumeroLinea, 10),
                Service: item.ServicioNumero.toString(),
                Quantity: this.round(parseFloat(item.CantidadACertificar), 3),
                ItemQuantity: item.Cantidad,
                UM: item.UM,
                ItemGrossPrice: item.ImporteString,
                GrossPrice: this.round(parseFloat((item.PrecioBruto / item.Cantidad).toString()), 2),
                Percentage: this.round(parseFloat(item.PorcentajeACertificar), 2).toString(),
                CertificationAmount: this.round(parseFloat(item.MontoACertificar), 2).toString(),
                ShortText: position.Descripcion,
                PlannedPackage: item.Id,
                PlannedLine: item.LINE_NO,
                Descripcion: item.Descripcion,
            }));

            const entrySheetServices = {
                Items: entrySheetServiceItems
            };

            this.entrySheetObjects.push({
                EntrySheetHeader: entrySheetHeader,
                EntrySheetServices: entrySheetServices
            });
        });
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
        item.MontoAnterior = (item.CantidadReal * item.Importe);
        return item.MontoAnterior;
    }

    //Redondeo de decimales
    round(num: number, decimals: number) {
        return Number(num.toFixed(decimals));
    }

    calcularPorcentajeAcumulado(rowData: any): number {
        let totalPorcentaje = (rowData.Porcentaje * 1) + (rowData.PorcentajeACertificar * 1);
        return Math.min(totalPorcentaje, 100);
    }


    // --------- CONFIGURACION DE COLUMNAS --------- //
    /**
     * Obtiene la configuración de columnas guardada en el session storage,
     * si no existe devuelve una configuración default.
     * @returns Array de columnas con sus propiedades.
     */
    private getColumnConfig(): Column[] {
        if (sessionStorage.getItem(this.colConfigName) == null) {
            const defaultColConfig = [{ name: 'anterior', visible: true }, { name: 'acumulado', visible: true }];
            this.saveColumnConfig(defaultColConfig);
        }
        let storedColConfig = sessionStorage.getItem(this.colConfigName);
        return JSON.parse(storedColConfig);
    }

    /**
     * Guarda la configuración en session storage.
     * @param colConfig
     */
    private saveColumnConfig(colConfig: any): void {
        sessionStorage.setItem(this.colConfigName, JSON.stringify(colConfig));
    }

    /**
    *  Actualiza la configuración de columnas.
    * @param colName
    * @param visible
    */
    private updateColumnConfig(colName: string, visible: boolean): void {
        this.colConfig = this.colConfig.map(col => {
            if (col.name === colName) {
                return { ...col, visible: visible };
            }
            return col;
        });
    }

    /**
     * Aplica la configuración de columnas a la tabla de alta
     * de certificaciones.
     * @param colConfig
     */
    private applyColumnConfig(colConfig: Column[]): void {
        setTimeout(() => {
            colConfig.forEach(col => {
                this.showHideColumn(col.name, col.visible);
            });
        }, 50); // Timeout necesario para que aparezca la tabla.
    }

    /**
     * Muestra/Oculta una columna.
     * @param colName Clase de css que identifica a la columna.
     * @param visible 
     */
    private showHideColumn(colName: string, visible: boolean): void {
        let colGroups = document.getElementsByClassName(colName) as HTMLCollectionOf<HTMLElement>;
        Array.from(colGroups).forEach(colGroup => {
            visible ? colGroup.classList.remove('hidden') : colGroup.classList.add('hidden');
        });

        // Necesario para mantener la estructura de la tabla
        if (colName === 'anterior') {
            this.colspanMonto = visible ? 10 : 7;
        }
    }

    toggleColumn(event) {
        let colName = event.target.value;
        let visible = event.target.checked;
        this.updateColumnConfig(colName, visible);
        this.applyColumnConfig(this.colConfig);
    }

    // --------- FIN CONFIGURACION DE COLUMNAS --------- //

    /**
     * No se puede comenzar la descripción con espacios en blanco
     * @param event 
     */
    limpiarComienzoConEspacios(event: KeyboardEvent): void {
        const inputElement = event.target as HTMLInputElement;
        const value = inputElement.value;
        if (/^\s/.test(value)) {
            inputElement.value = value.replace(/^\s+/, '');
        }
    }

    ngOnDestroy() {
        this.saveColumnConfig(this.colConfig);
    }

    validateValues() {
        if (this.fechaDocumento === null || this.fechaDocumento === undefined || this.fechaDocumento.toString() === '') {
            this.documentDateMsg = [];
            this.documentDateMsg.push({ severity: 'error', summary: '', detail: 'Por favor, ingrese una fecha de documento' });
            return false;
        }
        this.documentDateMsg = [];
        return true;
    }
}
