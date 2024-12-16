import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
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

  @Output() onLocalidadSeleccionada = new EventEmitter<any>();

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

  @Input() localidad_Id: number;
  @Input() localidadNombre: string;
  
  onChangeSearch(query: string) {
    if (query.length > 2) {
      this.unsubscribe();
      this.subscription = this.service.searchLocalidad(query).subscribe(
        (result) => {
          this.dataLocalidades = result;
        },
        (error) => {

        }
      );
    }
  }

  getLocalidadById(){
    this.unsubscribe();
      this.subscription = this.service.getLocalidadById(this.localidad_Id).subscribe(
        (result:{Nombre:string, Provincia:{Nombre:string}}) => {
          this.localidad = result.Nombre + " (" + result.Provincia.Nombre + ")";
        },
        (error) => {
        }
      );
  }


  selectEvent(item: { LocalidadId: number; }) {
    this.localidadIdSeleccionada = item.LocalidadId;
    this.onLocalidadSeleccionada.emit(this.localidadIdSeleccionada)
  }
}