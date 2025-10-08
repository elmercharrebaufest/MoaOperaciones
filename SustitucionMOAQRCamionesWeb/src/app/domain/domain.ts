// Domain services contain pure business logic
import { Injectable } from '@angular/core';
import { Greeting } from './entities';


@Injectable({ providedIn: 'root' })
export class DomainService {
  makeGreeting(name = 'Angularista'): Greeting {
    return { message: `Hola, ${name}!`, author: 'CleanArchitecture' };
  }
}