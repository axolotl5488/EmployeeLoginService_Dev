import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/Observable/throw';
import { Observable } from 'rxjs/Observable';
import { ResultStatus } from '../AppModel/appmodel_Model'
import { AppCommon } from '../AppCommon/AppCommon'
import {
    EmployeeleaveList_Detail, EmployeeleaveList_Response, EmployeeleaveList_request
} from '../AppModel/EmployeeLeave_Model'


@Injectable()
export class EmployeeLeave_Service {
    constructor(private _HttpClient: HttpClient) { }

    EmployeeLeaveList(model: EmployeeleaveList_request): Observable<EmployeeleaveList_Response> {
        return this._HttpClient.post(AppCommon.APIURL + "/EmployeeLeaveList", model).map(x => <EmployeeleaveList_Response>x).catch(this.httperrorHandle);
    }

    httperrorHandle(error: HttpErrorResponse) {
        return Observable.throw(error);
    }
}
