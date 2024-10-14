
import { Component } from '@angular/core';
import { ListBaseComponent } from '../common/base-components/list-base-component';
import { Seccion } from '../common/models/seccion';
import { FloatMsgService } from '../common/services/FloatMsgService';
import { ModalService } from '../common/services/ModalService';
import { NavService } from '../common/services/NavService';
import { SecurityService } from '../common/services/SecurityService';
import { SessionDataService } from '../common/services/SessionDataService';
import { CursosService } from './cursos.service';
import { BaseComponent } from '../common/base-components/base-component';
import { CURSOS_BASE_PATH, EstadoCurso } from '../common/models/cursos/Curso';
import { Permiso } from '../common/enums/Permisos';
import { FormControl } from '@angular/forms';

@Component({
    template: ``,
})
export class CursosBaseComponent extends ListBaseComponent {
    nombreFiltro = new FormControl();
    constructor(protected service: CursosService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }
    ngOnInit() {
        this.setTabs();
        let listNavegacion = [
            new Seccion(`/${CURSOS_BASE_PATH}/mis-cursos`, CURSOS_BASE_PATH, 'Mis Cursos'),
        ]
        if (this.isAuthorized(Permiso.AdministrarCursos)) {
            listNavegacion = [
                ...listNavegacion,
                new Seccion(`/${CURSOS_BASE_PATH}/administrar-cursos`, CURSOS_BASE_PATH, 'Administrar Cursos'),
            ]
        }

        this.navService.setSeccionList(listNavegacion);
        this.extraOnInit();
    }

    extraOnInit() { }
    estadosCursos = EstadoCurso
}