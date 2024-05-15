import { Component, Input, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { BehaviorSubject, Subscription, pipe } from 'rxjs';
import { AsignarAlumnosResDto, CursoDto } from '../../../common/models/cursos/Curso';
import { CursosService } from '../../cursos.service';
import { FormControl } from '@angular/forms';
import { UsuarioService } from '../../../usuario/usuario.service';
import { debounceTime, filter, finalize } from 'rxjs/operators';
import { BaseComponent } from '../../../common/base-components/base-component';
import { NavService } from '../../../common/services/NavService';
import { SecurityService } from '../../../common/services/SecurityService';
import { FloatMsgService } from '../../../common/services/FloatMsgService';
import { ModalService } from '../../../common/services/ModalService';
import { MensajeComponent } from '../../../common/view-child/mensaje/mensaje.component';
import { SessionDataService } from '../../../common/services/SessionDataService';
import { SpinnerComponent } from '../../../common/view-child/spinner/spinner.component';

@Component({
  selector: 'app-asignar-cursos',
  templateUrl: './asignar-cursos.component.html',
  styleUrls: ['./asignar-cursos.component.css']
})
export class AsignarCursosComponent extends BaseComponent implements OnInit {
  @ViewChild("mensaje")
  mensajeComponent: MensajeComponent;
  @ViewChild("spinner")
  spinnerComponent: SpinnerComponent;
  @Input()
  curso?: BehaviorSubject<CursoDto | null>;
  display = false;

  asignandoUsuarios = false;
  mailUsuarios: string[] = [];
  opcionesMail: string[] = [];

  resultados?: AsignarAlumnosResDto[];

  controlBuscarUsuarios = new FormControl(null);

  subscriptionsLocal = new Subscription();

  constructor(private service: CursosService, private usuarioService: UsuarioService,
    protected sessionDataService: SessionDataService,
    navService: NavService, securityService: SecurityService,
    floatMsgService: FloatMsgService, modalService: ModalService
  ) {
    super(navService, securityService, floatMsgService, modalService)
  }

  ngOnInit() {
    this.subscriptionsLocal.add(
      this.curso.subscribe(value => {
        this.display = !!value;
        if (!this.display) {
          this.restartValues()
        }
      })
    )
    this.subscriptionsLocal.add(
      this.controlBuscarUsuarios.valueChanges.pipe(
        filter(mail => mail && mail.length >= 3),
        debounceTime(1000)
      ).subscribe(mail => {
        this.spinnerComponent.showIt();
        this.mensajeComponent.setMsgsEmpty()
        this.usuarioService.getMailUsuarios(mail)
          .pipe(finalize(() => this.spinnerComponent.hideIt()))
          .subscribe(res => {
            const mails = this.manejarApiResponse(res, this.sessionDataService, this.mensajeComponent);
            if (mails)
              this.opcionesMail = mails;
          })
      })
    )
  }

  cerrar() {
    this.curso.next(null)
  }

  asignarUsuarios() {
    this.mensajeComponent.setMsgsEmpty();
    this.spinnerComponent.showIt();
    this.asignandoUsuarios = true;
    this.service.asignar({
      CursoId: this.curso.value.Id,
      MailsUsuarios: this.mailUsuarios
    })
      .pipe(finalize(() => { this.spinnerComponent.hideIt(); this.asignandoUsuarios = false; }))
      .subscribe(res => {
        const resultados = this.manejarApiResponse(res, this.sessionDataService, this.mensajeComponent)
        this.resultados = resultados;
        window.dispatchEvent(new Event("resize"));
      })
  }
  toggleTodos(check: boolean) {
    if (check) {
      this.mailUsuarios = [
        ... this.mailUsuarios,
        ... this.opcionesMail
      ]
    }
    if (!check) {
      this.mailUsuarios = this.mailUsuarios.filter(
        mailSeleccionado => !this.opcionesMail.includes(mailSeleccionado)
      )
    }
  }
  get todasOpcionesSeleccionadas() {
    return this.opcionesMail.every(op => this.mailUsuarios.includes(op))
  }
  restartValues() {
    this.mailUsuarios = [];
    this.opcionesMail = [];
    this.controlBuscarUsuarios.setValue(null)
    this.resultados = undefined;
    this.mensajeComponent.setMsgsEmpty();
  }

  extraOnDestroy(): void {
    this.subscriptionsLocal.unsubscribe()
  }
}
