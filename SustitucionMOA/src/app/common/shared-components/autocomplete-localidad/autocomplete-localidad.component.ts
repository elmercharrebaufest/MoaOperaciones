import { Component, EventEmitter, Input, OnInit, Output, SimpleChanges } from '@angular/core';
import { BaseComponent } from '../../base-components/base-component';
import { FloatMsgService } from '../../services/FloatMsgService';
import { ModalService } from '../../services/ModalService';
import { NavService } from '../../services/NavService';
import { SecurityService } from '../../services/SecurityService';
import { SessionDataService } from '../../services/SessionDataService';
import { AutocompleteLocalidadService } from './autocomplete-localidad.service';

@Component({
  selector: 'app-autocomplete-localidad',
  templateUrl: './autocomplete-localidad.component.html',
  styleUrls: ['./autocomplete-localidad.component.css'],
  providers: [AutocompleteLocalidadService],
})

export class AutocompleteLocalidadComponent extends BaseComponent implements OnInit {
  
  @Input() localidad_Id: number;
  @Input() localidadNombre: string;

  @Input() localidadPreseleccionada: { IdLocalidad: number, NombreLocalidad: string };
  
  @Output() onLocalidadSeleccionada = new EventEmitter<any>();
  @Output() nombreLocalidadSeleccionada = new EventEmitter<string>();

  constructor(protected service: AutocompleteLocalidadService, protected navService: NavService,
    protected sessionDataService: SessionDataService, protected securytiService: SecurityService,
    protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
    super(navService, securytiService, floatMsgService, modalService);
  }

  dataLocalidades = [];
  localidadIdSeleccionada: number;
  keyword = "Nombre";
  localidad: string;
  autocompleteNotFoundText = "No encontrado";
  
  ngOnInit() {

    setTimeout(() => {
      var autocompletesLocalidad = document.querySelectorAll('[placeholder="Localidad"]');
      for (let i = 0; i < autocompletesLocalidad.length; i++) {
        autocompletesLocalidad[i].setAttribute("autocomplete", "chrome-off");
      }
    }, 1000);

    if(this.localidad_Id != undefined && this.localidad_Id != null)
    {
      this.getLocalidadById();
    }
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes.localidadPreseleccionada) {
      this.preseleccionarLocalidad(changes.localidadPreseleccionada.currentValue);
    }
  }
  
  onChangeSearch(query: string) {
    if (query.length > 2) {
      this.unsubscribe();
      this.subscription = this.service.searchLocalidad(query).subscribe(
        (result) => {
          this.dataLocalidades = result;
        },
        (error) => { console.error(error); }
      );
    }
  }

  getLocalidadById(){
    this.unsubscribe();
      this.subscription = this.service.getLocalidadById(this.localidad_Id).subscribe(
        (result:{Nombre:string, Provincia:{Nombre:string}}) => {
          this.localidad = result.Nombre + " (" + result.Provincia.Nombre + ")";
        },
        (error) => { console.error(error); }
      );
  }

  selectEvent(item: { LocalidadId: number, Nombre: string }) {
    this.localidadIdSeleccionada = item.LocalidadId;
    this.onLocalidadSeleccionada.emit(this.localidadIdSeleccionada)
    this.nombreLocalidadSeleccionada.emit(item.Nombre);
  }

  preseleccionarLocalidad(localidadPreseleccionada: { IdLocalidad: number, NombreLocalidad: string }) {
    if (localidadPreseleccionada) {
      this.localidad = localidadPreseleccionada.NombreLocalidad;
      this.localidad_Id = localidadPreseleccionada.IdLocalidad;
    }
  }
}