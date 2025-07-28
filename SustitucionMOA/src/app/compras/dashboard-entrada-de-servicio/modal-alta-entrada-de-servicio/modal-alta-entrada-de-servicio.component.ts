import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { ComprasService } from '../../compras.service';
import { ConfirmationService, Message, MessageService } from 'primeng/api';
import { FormArray, FormGroup, FormsModule } from '@angular/forms';
import { SessionDataService } from '../../../common/services/SessionDataService';
import { FloatMsgService } from '../../../common/services/FloatMsgService';
import { BlockUI, NgBlockUI } from 'ng-block-ui';

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
    errorCallService: string = "Actualmente estamos experimentando problemas técnicos con nuestro servicio. Nuestro equipo ya está trabajando para resolverlo lo antes posible.Por favor, intente nuevamente más tarde.";
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
    @Input() formularioResumenCertificacion: FormGroup;
    @Output() closeModal = new EventEmitter<void>();
    @Output() closeDialog = new EventEmitter<void>();

    @Output() enviarMensajeGrilla = new EventEmitter();

    @ViewChild('fileInput') fileInput: any;
    @BlockUI() blockUI: NgBlockUI;

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
    get descriptions(): FormArray {
        return this.formularioResumenCertificacion.get('descriptions') as FormArray;
    }

    constructor(protected service: ComprasService,
        private confirmationService: ConfirmationService,
        protected sessionDataService: SessionDataService,
        protected floatMsgService: FloatMsgService,
        private messageService: MessageService
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
        this.colConfig = this.getColumnConfig();
    }

    ngAfterViewInit(): void {
    }

    ngAfterContentInit() {
        this.entrySheetObjects = [];
        this.agruparItemPorPosicion(this.itemSelected);
        this.itemsAgrupadosPorPosicion = this.orderBy(this.itemsAgrupadosPorPosicion, 'NroPosicion');
        this.applyColumnConfig(this.colConfig);
    }

    agruparItemPorPosicion(items) {
        this.itemsAgrupadosPorPosicion = items.reduce((prev, { NroPosicion, NroSolP, posicionDescripcion, ...Items }) => {
            const id = prev.findIndex((item) => item.NroPosicion === NroPosicion);
            if (id >= 0) {
                prev[id].MontoTotalACertificar = prev[id].MontoTotalACertificar + (Items.CantidadACertificar * Items.Importe);
                prev[id].Descripcion.trim();
                prev[id].Items.push(Items);
            } else {
                prev.push({ NroPosicion, Descripcion: Items.Descripcion.trim(), Items: [Items], MontoTotalACertificar: (Items.CantidadACertificar * Items.Importe), NroSolP, posicionDescripcion })
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
            return `${moneda} ${montoTotalGeneral.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`;
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
            let respIdAdjuntos = { data: [] };
            this.certificarState = true;
            this.buildEntrySheet();
            
            let items = this.itemSelected;
            items = items.map(element => {
                element.EntradasServicio = [];
                return element;
            });

            if(this.uploadedFiles.length > 0){
                this.blockUI.start('Adjuntando archivos...');
                respIdAdjuntos = await this.service.AdjuntarArchivosCertificacion(this.uploadedFiles).toPromise();
                this.blockUI.stop();
            }

            this.blockUI.start('Confirmando la certificación...');
            this.service.CrearEntradaServicio(this.entrySheetObjects, items, respIdAdjuntos.data).subscribe(
                (response) => {
                    this.mensajeError = '';
                    let resultMsj: string[] = [];
                    let msjTypes: string[] = [];
                    if (response.logout == true) {
                        this.sessionDataService.logout();
                    }
                    else {
                        if (response.error) {
                            resultMsj.push(`<li>${response.error}</li>`);
                            msjTypes.push('E');
                        }
                        else {
                            response.data.forEach(element => {
                            if (!element) {
                                this.mensajeError = "Ha ocurrido un error por favor inténtelo nuevamente más tarde."
                                resultMsj.push("<li>Ha ocurrido un error por favor inténtelo nuevamente más tarde.</li>");
                            }
                            else {
                                let msj = element.Message.startsWith("Sólo es posible contabilizar en ") ||
                                    element.Message.startsWith("Contabilice en ") ?
                                    "El período se encuentra cerrado, por favor contabilice en el periodo actual." : element.Message;

                                resultMsj.push("<li>" + msj + "</li>");

                                if (!msjTypes.includes(element.Type)) {
                                    msjTypes.push(element.Type);
                                }
                            }});
                        }
                    }
                    this.blockUI.stop();
                    this.mensajeError = resultMsj.join("");
                    this.confirmationService.confirm({
                        message: "<ul>" + this.mensajeError + "</ul>",
                        accept: () => this.cerrarMensajes(msjTypes),
                        reject: () => this.cerrarMensajes(msjTypes),
                        rejectVisible: false
                    });
                    this.certificarState = false;
                },
                (error) => {
                    this.blockUI.stop();
                    this.confirmationService.confirm({
                        message: error.status === 500 ? this.errorCallService : error.error.Message,
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
        const items = this.orderBy(this.itemSelected, 'NroPosicion');

        this.service.buildReportES(items).subscribe(
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

    onDescripcionChange(position: any, value: string) {
        const item = this.itemSelected.find(item => item.NroPosicion === position.NroPosicion);

        if (item) {
            item.posicionDescripcion = value;
        }

        this.itemsAgrupadosPorPosicion.forEach((posicion) => {
            if (posicion.NroPosicion === position.NroPosicion) {
                posicion.Items.forEach(itemAgrupado => {
                    itemAgrupado.posicionDescripcion = value;
                });
            }
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

        class PositionData {
            positionId: string;
            SolPedNumber: string;
        }

        let ArrayOfSolpeds: PositionData[] = [];

        this.elementSelected.Posiciones.forEach((posicion) => {
            let hasSelectedItems = false;
            let positionData: PositionData = {
                positionId: "",
                SolPedNumber: "",
            };

            for (let item of posicion.Items) {
                if (item.isSelected) {
                    hasSelectedItems = true;
                    break;
                }
            }

            if (hasSelectedItems) {
                positionData.positionId = posicion.NumeroPosicion.toString();
                positionData.SolPedNumber = posicion.NumeroSolp;
                ArrayOfSolpeds.push(positionData);
            }
        });


        this.itemsAgrupadosPorPosicion.forEach((position, index) => {

            //let solpedNumbers = this.elementSelected.Posiciones.filter(posicion => posicion.isSelected).map(posicion => posicion.NumeroSolp);
            //Encontrar SolPed desde ArrayOfSolpeds
            let matchedPosition = ArrayOfSolpeds.find(x => x.positionId === position.NroPosicion);

            let solPedAsociada = "";
            if (matchedPosition) {
                solPedAsociada = matchedPosition.SolPedNumber;
            }

            const entrySheetHeader = {
                SolPedNumber: solPedAsociada,
                MontoTotalACertificar: this.round(this.totalMontoCertificar, 2).toString(),
                PaqueteNumero: position.NroPosicion.toString(),
                Descripcion: this.descriptions.at(index).get('description').value.trim(),
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
                GrossPrice: this.round(parseFloat((item.PrecioBruto).toString()), 2),
                Percentage: this.round(parseFloat(item.PorcentajeACertificar), 2).toString(),
                CertificationAmount: this.round(parseFloat(item.MontoACertificar), 2).toString(),
                ShortText: item.posicionDescripcion || position.posicionDescripcion,
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
        // Limpiar variables
    this.itemsAgrupadosPorPosicion = [];
    this.mensajeError = '';
    this.uploadedFiles = [];
    this.entrySheetObjects = [];
    }

    validateValues() {
        this.documentDateMsg = [];

        if (this.fechaDocumento === null || this.fechaDocumento === undefined || this.fechaDocumento.toString() === '') {
            this.documentDateMsg.push({ severity: 'error', summary: '', detail: 'Por favor, ingrese una fecha de documento' });
            return false;
        }

        if (this.referencia != undefined && this.referencia != null && this.referencia.length > 0) {
            this.referencia = this.referencia.replace('r', 'R');
            if (!(/^([0-9]{4})(R{1})([0-9]{8})$/i.test(this.referencia))) {
                this.documentDateMsg.push({ severity: 'error', summary: '', detail: 'Ingrese una referencia remito válida: 4 dígitos + R + 8 dígitos.' });
                return false;
            }
        }

        return true;
    }

    clearMessage() {
        setTimeout(() => {
          this.messageService.clear();
        }, 10000)
      }


    uploadedFiles: File[] = [];
    maxSizeFile = 10 * 1024 * 1024; // 10 MB
    allowedTypes = ['application/pdf', 
        'application/vnd.ms-excel', 
        'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet', 
        'application/vnd.ms-outlook', 
        'application/octet-stream', 
        'application/x-msg'];

    allowedExtensions = ['.pdf', '.xls', '.xlsx', '.msg'];

    onFileSelected(event: any) {
      const files: FileList = event.target.files;
      let totalSize = this.uploadedFiles.reduce((acc, file) => acc + file.size, 0);
  
      for (let i = 0; i < files.length; i++) {
        const file = files[i];
        if (!this.isValidFileType(file)) {
            this.messageService.add({ severity: 'error', summary: `${file.name} Archivo invalido.`, detail: 'Solo se permiten archivos .pdf, .xls, .msg.' })
            this.clearMessage();
          continue;
        }
        if (totalSize + file.size > this.maxSizeFile) {
            this.messageService.add({ severity: 'error', summary: 'Tamaño excedido 10 MB.', detail: 'El limite de carga de archivos es de 10 MB.' })
            this.clearMessage();
          continue;
        }
        this.uploadedFiles.push(file);
        totalSize += file.size;
      }
  
      this.updateFileInput();
    }
  

    isValidFileType(file: File): boolean {
        const fileTypeValid = this.allowedTypes.includes(file.type);
        const fileExtensionValid = this.allowedExtensions.some(ext => file.name.endsWith(ext));
        return fileTypeValid || fileExtensionValid;
    }
  
    removeFile(index: number) {
      this.uploadedFiles.splice(index, 1);
      this.updateFileInput();
    }
  
    updateFileInput() {
      const dt = new DataTransfer();
      this.uploadedFiles.forEach(file => dt.items.add(file));
      this.fileInput.nativeElement.files = dt.files;
    }
  
    uploadFiles() {
      const formData = new FormData();
      for (let file of this.uploadedFiles) {
        formData.append('files', file, file.name);
      }
    }
}
