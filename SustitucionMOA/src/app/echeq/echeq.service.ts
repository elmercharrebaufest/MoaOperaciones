
import {throwError as observableThrowError,  Observable } from 'rxjs';
import {timeoutWith, map} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { BaseService } from './../common/services/BaseService';
import { HttpParams } from '@angular/common/http';
 
@Injectable()
export class EcheqService extends BaseService {}



