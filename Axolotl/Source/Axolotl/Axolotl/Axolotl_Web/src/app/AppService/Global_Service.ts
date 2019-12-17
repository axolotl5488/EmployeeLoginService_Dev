import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/Observable/throw';
import { Observable } from 'rxjs/Observable';
import { ResultStatus } from '../AppModel/appmodel_Model'
import { AppCommon } from '../AppCommon/AppCommon'
import {
  GetCompanyList_drp_response
} from '../AppModel/Global_Models'


@Injectable()
export class Global_Service {
  constructor(private _HttpClient: HttpClient) { }

  GetCompanyList_drp(): Observable<GetCompanyList_drp_response> {
    return this._HttpClient.post(AppCommon.APIURL + "/GetCompanyList_drp", null).map(x => <GetCompanyList_drp_response>x).catch(this.httperrorHandle);
  }

  

  httperrorHandle(error: HttpErrorResponse) {
    return Observable.throw(error);
  }
}
