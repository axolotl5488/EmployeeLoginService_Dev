import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/Observable/throw';
import { Observable } from 'rxjs/Observable';
import { ResultStatus } from '../AppModel/appmodel_Model'
import { AppCommon } from '../AppCommon/AppCommon'
import {
    GetCompany_Detail, GetCompany_Response, GetCompanyDetail_Request, GetCompanyDetail_Response, ManageCompany_Request,ManageCompany_Response
} from '../AppModel/Company_Models'


@Injectable()
export class Company_Service {
    constructor(private _HttpClient: HttpClient) { }

    GetCompanyList(): Observable<GetCompany_Response> {
        return this._HttpClient.post(AppCommon.APIURL + "/GetCompanyList", null).map(x => <GetCompany_Response>x).catch(this.httperrorHandle);
    }

    ManageCompany(model: ManageCompany_Request): Observable<ManageCompany_Response> {
        return this._HttpClient.post(AppCommon.APIURL + "/ManageCompany", model).map(x => <ManageCompany_Response>x).catch(this.httperrorHandle);
    }

    GetCompanyDetail(model: GetCompanyDetail_Request): Observable<GetCompanyDetail_Response> {
        return this._HttpClient.post(AppCommon.APIURL + "/GetCompanyDetail", model).map(x => <GetCompanyDetail_Response>x).catch(this.httperrorHandle);
    }

    httperrorHandle(error: HttpErrorResponse) {
        return Observable.throw(error);
    }
}
