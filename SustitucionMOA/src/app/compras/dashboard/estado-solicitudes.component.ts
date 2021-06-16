// import { Component } from '@angular/core';
// import { Router, ActivatedRoute } from '@angular/router';
// import { ListBaseComponent } from '../../common/base-components/list-base-component'
// import { SessionDataService } from '../../common/services/SessionDataService';
// import { SecurityService } from '../../common/services/SecurityService';
// import { NavService } from '../../common/services/NavService';
// import { FloatMsgService } from '../../common/services/FloatMsgService';
// import { ModalService } from '../../common/services/ModalService';
// import { ComprasService } from '../compras.service';


// declare var $: any;

// @Component({
//     selector: 'estado-solicitudes',
//     templateUrl: `estado-solicitudes.component.html`,
//     styleUrls: ['../compras.component.css'],
//     providers: [ComprasService]
// })
// export class EstadoSolicitudesComponent extends ListBaseComponent {

//     constructor(protected service: ComprasService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService, protected route: ActivatedRoute, protected router: Router) {
//         super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
//     }



//     ngOnInit() {
//         this.navService.setSeccionList([]);

//     }

    
// }