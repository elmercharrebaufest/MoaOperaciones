import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ListBaseComponent } from '../../../common/base-components/list-base-component';
import { FloatMsgService } from '../../../common/services/FloatMsgService';
import { ModalService } from '../../../common/services/ModalService';
import { NavService } from '../../../common/services/NavService';
import { SecurityService } from '../../../common/services/SecurityService';
import { SessionDataService } from '../../../common/services/SessionDataService';
import { SpinnerComponent } from '../../../common/view-child/spinner/spinner.component';
import { ComprasService } from '../../compras.service';
import { SelectItem } from 'primeng/api';

@Component({
    selector: 'app-filtro-dashboard-comprador',
    templateUrl: `filtro-dashboard-comprador.component.html`,
    styleUrls: ['../../compras.component.css',
        './filtro-dashboard-comprador.component.css']

})
export class FiltroDashboardCompradorComponent extends ListBaseComponent {

    //#region Variables 
    nroSolp: string;
    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;
    usuario: string;
    usuarioFiltro: SelectItem[];
    selectUsuario: number | null;
    estadoSolpItem: SelectItem[];
    selectEstadoSolp: string[] = [];
    grupoComprasFiltro: SelectItem[];
    selectGrupoCompras: string[] = [];
    centroFiltro: SelectItem[];
    selectCentro: string[] = [];
    usuariosResult: any;
    //#endregion

    constructor(protected service: ComprasService, protected navService: NavService,
        protected sessionDataService: SessionDataService, protected securityService: SecurityService,
        protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        protected route: ActivatedRoute, protected router: Router, private formBuilder: FormBuilder) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
        this.usuario = sessionStorage.getItem("username");
        this.onBuscar();
    }

    ngOnInit(): void {
    }

    ngAfterViewInit(): void {
        this.getCombos();
    }

    getCombos() {
        try {
            this.subscription = this.service.getCombos().subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.usuariosResult = result.Usuarios;
                        this.estadoSolpItem = [];
                        this.usuarioFiltro = [];
                        this.centroFiltro = [];
                        this.grupoComprasFiltro = [];
                        result.EstadosSolpSap.forEach(e => this.estadoSolpItem.push({
                            label: e.Descripcion, value: e.Id
                        }));
                        result.Usuarios.forEach(x => x.forEach(d => this.usuarioFiltro.push({
                            label: d.Mail, value: d.Id
                        })));
                        result.Centro.forEach(c => this.centroFiltro.push({
                            label: c.Codigo + " - " + c.Descripcion, value: c.Id
                        }));
                        result.GrupoCompras.forEach(gc => this.grupoComprasFiltro.push({
                            label: gc.Codigo + " - " + gc.Descripcion, value: gc.Id
                        }));
                    }
                },
                error => {
                    this.floatMsgService.setErrorMsg(error.message);
                }
            );
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    }

    onBuscar() {
        this.service.getListarSolpCompras(1, 10, "", "", this.nroSolp, this.selectEstadoSolp.join(","),
            this.selectUsuario, this.selectCentro.join(","), this.selectGrupoCompras.join(","));
    }
}