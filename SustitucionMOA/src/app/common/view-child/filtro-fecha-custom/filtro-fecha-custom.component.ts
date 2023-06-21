import { Component, Output, EventEmitter, ViewChild, OnInit, Input } from '@angular/core';
import { Formatter } from './../../formatter/Formatter';
import { DropdownComponent, DropdownOption } from './../dropdown/dropdown.component';
declare var $: any;

@Component({
  selector: 'app-filtro-fecha-custom',
  templateUrl: './filtro-fecha-custom.component.html',
  styleUrls: ['./filtro-fecha-custom.component.css']
})
export class FiltroFechaCustomComponent implements OnInit {


  @Output() ClickEvent = new EventEmitter();
  @Input()leyenda?:string;

  @ViewChild(DropdownComponent)
  private dropdownComponent: DropdownComponent;

  periodo: string;
  fecha_inicio: string;
  fecha_fin: string;

  constructor() {
      this.dropdownComponent = new DropdownComponent();
      this.setPeriodoInitial(sessionStorage.getItem("periodo") ? sessionStorage.getItem("periodo") : "1");
  }

  ngOnInit() {
      this.dropdownComponent.setSelectItem(sessionStorage.getItem("periodo") ? sessionStorage.getItem("periodo") : "1");
  }

  ngAfterViewInit(): void {

      $(document).on("mouseover", '.form_datetime1', function () {
          $(".form_datetime1").datetimepicker({
              format: 'dd/mm/yyyy',
              language: 'es',
              weekStart: 1,
              todayBtn: 1,
              autoclose: 1,
              todayHighlight: 1,
              startView: 2,
              forceParse: 0,
              showMeridian: 1,
              pickTime: false,
              minView: 2,
              maxView: 4
          });
      });

      $(document).on("mouseover", '.form_datetime2', function () {
          $(".form_datetime2").datetimepicker({
              format: 'dd/mm/yyyy',
              language: 'es',
              weekStart: 1,
              todayBtn: 1,
              autoclose: 1,
              todayHighlight: 1,
              startView: 2,
              forceParse: 0,
              showMeridian: 1,
              pickTime: false,
              minView: 2,
              maxView: 4
          });
      });
  }

  setDropdownOptions(options: Array<DropdownOption>) {
      this.dropdownComponent.options = options;
  }
  private reformatDate(dateStr){
    const dArr = dateStr.split("-");
    let reformatDateString = '';
    if(dArr!=null && dArr.length > 2)
      reformatDateString = dArr[2]+ "/" +dArr[1]+ "/" +dArr[0];
      else
      reformatDateString = dateStr;
    return reformatDateString;
  }
  filterByDate(fechaHoraInicio: string, fechaHoraFin: string) {
      var fechaInicio = fechaHoraInicio;
      var fechaFin = fechaHoraFin;

      if (fechaHoraInicio != undefined) {
          var fechaHoraInicioArray = fechaHoraInicio.split(" ");
          if (fechaHoraInicioArray.length > 0)
              fechaInicio = fechaHoraInicioArray[0];
      }

      if (fechaHoraInicio != undefined) {
          var fechaHoraFinArray = fechaHoraFin.split(" ");
          if (fechaHoraFinArray.length > 0)
              fechaFin = fechaHoraFinArray[0];
      }
      this.setFechaIncio(Formatter.reformatDate(fechaInicio));
      this.setFechaFin(Formatter.reformatDate(fechaFin));
      sessionStorage.setItem("fechaInicioCustom", fechaInicio);
      sessionStorage.setItem("fechaInicioCustom", fechaFin);
      this.ClickEvent.emit();
      return false;
  }

  setPeriodoInitial(periodo: string) {
      if (periodo == "4") {
          let fechaInicio = sessionStorage.getItem("fechaInicioCustom");
          let fechaFin = sessionStorage.getItem("fechaInicioCustom");
          this.setFechaIncio(fechaInicio!=null ? fechaInicio : Formatter.DateToStringCustom(new Date(new Date().setDate(new Date().getDate() - 1))));
          this.setFechaFin(fechaFin!=null ? fechaFin : Formatter.DateToStringCustom(new Date()));
          sessionStorage.setItem("fechaInicioCustom", this.fecha_inicio);
          sessionStorage.setItem("fechaFinCustom", this.fecha_fin);
          this.periodo = periodo;
      } else {
          this.setPeriodo(periodo);
      }
  }

  setPeriodo(periodo: string) {

      switch (periodo) {

          case "2":
              this.setFechaIncio(Formatter.DateToStringCustom(new Date(new Date().setDate(new Date().getDate() - 7))));
              this.setFechaFin(Formatter.DateToStringCustom(new Date()));
              this.guardarFechasYSetarPeriodo(periodo);
              this.ClickEvent.emit();
              break;

          case "3":
              this.setFechaIncio(Formatter.DateToStringCustom(new Date(new Date().setDate(new Date().getDate() - 30))));
              this.setFechaFin(Formatter.DateToStringCustom(new Date()));
              this.guardarFechasYSetarPeriodo(periodo);
              this.ClickEvent.emit();
              break;

          case "5":
              this.setFechaIncio(Formatter.DateToStringCustom(new Date(new Date().setDate(new Date().getDate() - 60))));
              this.setFechaFin(Formatter.DateToStringCustom(new Date()));
              this.guardarFechasYSetarPeriodo(periodo);
              this.ClickEvent.emit();
              break;

          case "4":
              this.setFechaIncio(Formatter.DateToStringCustom(new Date(new Date().setDate(new Date().getDate() - 1))));
              this.setFechaFin(Formatter.DateToStringCustom(new Date()));
              this.guardarFechasYSetarPeriodo(periodo);
              break;

          case "1":
          default:
              this.setFechaIncio(Formatter.DateToStringCustom(new Date(new Date().setDate(new Date().getDate() - 1))));
              this.setFechaFin(Formatter.DateToStringCustom(new Date()));
              this.guardarFechasYSetarPeriodo(periodo);
              this.ClickEvent.emit();
              break;
      }

  }

  guardarFechasYSetarPeriodo(periodo: string) {
      this.dropdownComponent.setSelectItem(periodo);
      sessionStorage.setItem("periodo", periodo);
      this.periodo = periodo;
  }

  updateFechaFin(event: any) {
      this.fecha_inicio = "a";
  }

  setFechaIncio(fecha: string) {
      this.fecha_inicio = fecha;
  }

  setFechaFin(fecha: string) {
      this.fecha_fin = fecha;
  }

  getPeriodo() {
      return this.periodo;
  }

  getFechaIncio() {
      return this.fecha_inicio;
  }

  getFechaFin() {
      return this.fecha_fin;
  }


}
