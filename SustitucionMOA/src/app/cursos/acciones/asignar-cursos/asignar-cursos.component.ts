import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { BehaviorSubject, Subscription } from 'rxjs';
import { CursoDto } from '../../../common/models/cursos/Curso';
import { CursosService } from '../../cursos.service';
import { FormControl } from '@angular/forms';
import { UsuarioService } from '../../../usuario/usuario.service';
import { debounceTime } from 'rxjs/operators';
import { BaseComponent } from '../../../common/base-components/base-component';
import { NavService } from '../../../common/services/NavService';
import { SecurityService } from '../../../common/services/SecurityService';
import { FloatMsgService } from '../../../common/services/FloatMsgService';
import { ModalService } from '../../../common/services/ModalService';
import { MensajeComponent } from '../../../common/view-child/mensaje/mensaje.component';
import { SessionDataService } from '../../../common/services/SessionDataService';

@Component({
  selector: 'app-asignar-cursos',
  templateUrl: './asignar-cursos.component.html',
  styleUrls: ['./asignar-cursos.component.css']
})
export class AsignarCursosComponent extends BaseComponent implements OnInit {
  @ViewChild("mensaje")
  mensajeComponent: MensajeComponent;
  @Input()
  curso?: BehaviorSubject<CursoDto | null>;
  display = false;

  mailUsuarios: string[] = [];
  opcionesMail: string[] = [];

  resultados?: { [key: string]: boolean }[];

  controlBuscarUsuarios = new FormControl(null);

  subscriptions = new Subscription();

  constructor(private service: CursosService, private usuarioService: UsuarioService,
    protected sessionDataService: SessionDataService,
    navService: NavService, securityService: SecurityService,
    floatMsgService: FloatMsgService, modalService: ModalService
  ) {
    super(navService, securityService, floatMsgService, modalService)
  }

  ngOnInit() {
    this.subscriptions.add(
      this.curso.subscribe(value => this.display = !!value)
    )
    this.subscriptions.add(
      this.controlBuscarUsuarios.valueChanges.pipe(
        debounceTime(1000)
      ).subscribe(mail => {
        this.usuarioService.getMailUsuarios(mail).subscribe(res => {
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
    this.service.asignar({
      CursoId: this.curso.value.CursoId,
      MailsUsuarios: this.mailUsuarios
    }).subscribe(res => {
      const resultados = this.manejarApiResponse(res, this.sessionDataService, this.mensajeComponent)
      this.resultados = resultados;
    })
  }
}
