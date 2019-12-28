import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/Observable/throw';
import { Observable } from 'rxjs/Observable';
import { ResultStatus } from '../AppModel/appmodel_Model'
import { AppCommon } from '../AppCommon/AppCommon'
import {
  EmployeePunchList_Response, EmployeePunchList_Detail, GetEmployeePunchDetailWeb_request, GetEmployeePunchDetailWeb_response, PunchTask_Model
} from '../AppModel/EmployePunch_Model'


@Injectable()
export class EmployeePunch_Service {
    constructor(private _HttpClient: HttpClient) { }

    EmployeePunchList(): Observable<EmployeePunchList_Response> {
        return this._HttpClient.post(AppCommon.APIURL + "/EmployeePunchList", null).map(x => <EmployeePunchList_Response>x).catch(this.httperrorHandle);
  }

  GetEmployeePunchDetail(model: GetEmployeePunchDetailWeb_request): Observable<GetEmployeePunchDetailWeb_response> {
    return this._HttpClient.post(AppCommon.APIURL + "/GetEmployeePunchDetail", model).map(x => <GetEmployeePunchDetailWeb_response>x).catch(this.httperrorHandle);
  }

    httperrorHandle(error: HttpErrorResponse) {
        return Observable.throw(error);
    }
}
