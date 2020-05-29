declare let ga: Function;
import { Component, Injector } from '@angular/core';
import { Router, NavigationEnd } from "@angular/router";
import { ServiceLocator } from './common/services/ServiceLocator';


@Component({
    selector: 'my-app',
    templateUrl: `./app/app.component.html`
})
export class AppComponent {

    constructor(private injector: Injector, public router: Router) {
        ServiceLocator.injector = this.injector;
        this.router.events.subscribe(event => {
            if (event instanceof NavigationEnd) {
                try {
                    ga('set', 'page', event.urlAfterRedirects);
                    ga('send', 'pageview');
                }catch(e) { }
            }
        });

    }
}