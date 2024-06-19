import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { Subject, Subscription } from 'rxjs';
import { autoCompleteObject } from '../../listado-estado-certificaciones/listado-estado-certificaciones.component';
import { DropdownOption } from '../../../../common/view-child/dropdown/dropdown.component';


@Component({
    selector: 'aux-pannel',
    templateUrl: './aux-pannel.component.html',
    styleUrls: ['./aux-pannel.component.css']
})

export class AuxPannelComponent implements OnInit {
    constructor() { }

    ngOnInit(): void { }

    @Input() showOrHideAuxPanel: boolean = false;
    @Input() fechaSeleccionada: string = '1';
    @Input() proveedor: string = '';
    @Input() visible: boolean = false;
    @Output() obtenerESSap: EventEmitter<boolean> = new EventEmitter<boolean>();
    @Output() getFecha: EventEmitter<string> = new EventEmitter<string>();

    subscripciones: Subscription[] = [];
    proveedorSeleccionado: autoCompleteObject;
    fechaInicio = "";


    filtroFechas: Array<DropdownOption> = [
        new DropdownOption("1", "Últimos dos dias"),
        new DropdownOption("2", "Última semana"),
        new DropdownOption("3", "Último mes"),
      ];

    /**
    * Muestra/Oculta un panel según nombre de clase
    * que lo identifica.
    * Sólo un panel puede estar activo a la vez.
    * @param className
    */
    togglePanel(className: string): void {
        let panels = document.getElementsByClassName('aux-panel') as HTMLCollectionOf<HTMLElement>;
        Array.from(panels).forEach(panel => {
            if (panel.id === className && panel.classList.contains('hidden')) {
                panel.classList.remove('hidden');
            }
            else {
                panel.classList.add('hidden');
            }
        });
    }

    setDateByRange(event: string): void {
        this.getFecha.emit(event);
    }

    /**
      * filtro de busqueda de las entradas de servicio por rango de fechas.
      */
    onBuscar() {
        // MMSN-519: Colapsar fila expandida al activar un filtro.
        this.collapseExpandedRow();

        if (this.proveedorSeleccionado !== undefined) {
            this.proveedor = this.proveedorSeleccionado.CodigoProveedor;
        }
        else {
            this.proveedor = '';
        }

        this.obtenerESSap.emit(true);
    }


    /**
   * Colapsa la fila expandida.
   */
    collapseExpandedRow() {
        let elementExpanded = document.querySelector('.pi-chevron-down') as HTMLElement;
        if (elementExpanded != null) {
            elementExpanded.click();
        }
    }

}
