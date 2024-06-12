import { AfterContentInit, Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';

type Column = {
    name: string;
    visible: boolean;
};

@Component({
    selector: 'modal-aprobacion',
    templateUrl: 'modal-aprobacion.component.html',
    styleUrls: ['modal-aprobacion.component.css']
})

export class ModalAprobacionComponent implements OnInit, OnDestroy, AfterContentInit {
    colConfig = [];
    @Input() showModal: boolean = false;
    @Input() entradaServicioSeleccionada: certificacionES[] = [];
    @Output() closeModal = new EventEmitter<void>();
    @Output() aprobarES = new EventEmitter<boolean>();
    colConfigName: string = 'columnasAprobaciones';
    itemsAgrupadosPorPosicion: any[] = [];
    fechaDocumento: Date = new Date();
    fechaDocMin: Date = new Date();
    fechaDocMax: Date = new Date();
    fechaContabilizacion: Date = new Date();
    fechaContabilizacionMin: Date = new Date();
    fechaContabilizacionMax: Date = new Date();
    referencia: string = '';
    es: any = {
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
    monthNavStatus: boolean = false;
    totalMontoCertificar: number = 0;
    colspanMonto: number = 10;

    constructor() { }

    ngOnDestroy(): void {
        this.saveColumnConfig(this.colConfig);
    }

    ngOnInit() {
        this.setRangoFechaDocumento();
        this.setRangoFechaContabilizacion();
        this.calcularTotalMontoCertificar();
        this.entradaServicioSeleccionada = this.orderBy(this.entradaServicioSeleccionada, 'NroPosicion');
        this.colConfig = this.getColumnConfig();
    }

    ngAfterContentInit(): void {
        this.applyColumnConfig(this.colConfig);
    }
    /**
     * Ordena el array por el Nro de posicion.
     * @param array 
     * @param field 
     * @returns 
     */
    orderBy(array: any[], field: string): any[] {
        return array.sort((a, b) => a[field] - b[field]);
    }

    /**
     * Calcula el total del monto a certificar
     */
    calcularTotalMontoCertificar() {
        let total = 0;
        for (let item of this.entradaServicioSeleccionada) {
            total += item.MontoACertificar;
        }

        this.totalMontoCertificar = total;
    }

    /**
     * Setear rango de fecha documento.
     */
    setRangoFechaDocumento() {
        // Fecha mínima: Fecha Actual 5 años hacia atrás
        this.fechaDocMin = new Date(new Date().setFullYear(new Date().getFullYear() - 5));
    }

    /**
     * Setear rango de fecha contabilizada.
     */
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

    /**
     * Agrupa posiciones por items
     * @param items 
     */
    agruparItemPorPosicion(items) {
        this.itemsAgrupadosPorPosicion = items.reduce((prev, { NroPosicion, ...Items }) => {
            const id = prev.findIndex((item) => item.NroPosicion === NroPosicion);
            if (id >= 0) {
                prev[id].MontoTotalACertificar = prev[id].MontoTotalACertificar + (Items.CantidadACertificar * Items.Importe);
                prev[id].Items.push(Items);
            } else {
                prev.push({ NroPosicion, Descripcion: Items.Descripcion, Items: [Items], MontoTotalACertificar: (Items.CantidadACertificar * Items.Importe) })
            }
            return prev;
        }, []);
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
     * 
     * @param event 
     */
    toggleColumn(event) {
        let colName = event.target.value;
        let visible = event.target.checked;
        this.updateColumnConfig(colName, visible);
        this.applyColumnConfig(this.colConfig);
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
     * Calcula el monto total general.
     * @returns 
     */
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

    /**
     * Validacion de numero de referencia.
     * @param event 
     */
    validarReferenciaRemito(event) {
        let element = document.getElementById("ref");

        if (element.classList.contains('ng-dirty') && element.classList.contains('ng-invalid')) {
            element.classList.add('error');
        } else {
            element.classList.remove('error');
        }
    }

    /**
     * Aprobar Entrada de servicio
     */
    aprobarEntradaServicio() {
        this.aprobarES.emit(true);
    }

    /**
     * Mostrar modal de resumen.
     */
    showIt(): void {
        this.showModal = true;
    }

    /**
     * Ocultar modal de resumen.
     */
    hideIt(): void {
        this.showModal = false;
    }
}