import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'compras-celda-editable-ingresante',
  templateUrl: 'celda-editable.component.html',
  styleUrls: ['celda-editable.component.css']
})

export class CeldaEditableComponent implements OnInit {
  editarInfo: boolean = false;
  formularioActualizarInformacionIngresante: FormGroup | undefined;
  @Input() columnaEditar: string = '';
  @Input() procesando: boolean = false;
  @Input() value: string = '';
  @Output() infoEmitter: EventEmitter<string> = new EventEmitter<string>();
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
  fechaDocMin: Date = new Date();
  fechaDocMax: Date = new Date();

  constructor(private formBuilder: FormBuilder) { }

  ngOnInit() {
    this.setRangoFechaDocumento();
    switch (this.columnaEditar) {
      case 'Remito':
        this.formularioActualizarInformacionIngresante = this.formBuilder.group({
          info: [
            '',
            [
              Validators.required,
              Validators.pattern('^[0-9]{4}R[0-9]{8}$'),
              Validators.maxLength(13)
            ]
          ]
        });
        break;
      case 'DescripcionES':
        this.formularioActualizarInformacionIngresante = this.formBuilder.group({
          info: [
            '',
            [
              Validators.required,
              Validators.maxLength(40),
              Validators.pattern('^[a-zA-Z0-9 ]*$')
            ]
          ]
        });
        break;
      default:
        this.formularioActualizarInformacionIngresante = this.formBuilder.group({
          info: ['', Validators.required]
        });
        break;
    }
  }

  get f() {
    return this.formularioActualizarInformacionIngresante.controls;
  }

  mostrarFormularioEditarDescricionES(editarInfo: boolean, value: any): void {
    this.formularioActualizarInformacionIngresante.controls['info'].patchValue(value.trim());
    this.editarInfo = !editarInfo ? true : false;
  }

  actualizarInformacionIngresante(): void {
    let info: any = this.formularioActualizarInformacionIngresante.get('info').value.trim();
    if (info !== this.value) {
      if (this.columnaEditar === 'FechaDocumento') {
        info = this.dateFormatter(info);
      }
      this.infoEmitter.emit(info);
    }
    this.editarInfo = false;
  }

  setRangoFechaDocumento() {
    // Fecha máxima: Fecha actual
    this.fechaDocMax = new Date();
    // Fecha mínima: Fecha Actual 5 años hacia atrás
    this.fechaDocMin = new Date(new Date().setFullYear(new Date().getFullYear() - 5));
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

}