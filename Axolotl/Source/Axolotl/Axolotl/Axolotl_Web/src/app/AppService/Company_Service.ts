import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/Observable/throw';
import { Observable } from 'rxjs/Observable';
import { ResultStatus, AppCommonResponse } from '../AppModel/appmodel_Model'
import { AppCommon } from '../AppCommon/AppCommon'
import {
  GetCompany_Detail, GetCompany_Response, GetCompanyDetail_Request, GetCompanyDetail_Response, ManageCompany_Request, ManageCompany_Response,
  GetCompanyLocaitonList_Detail, GetCompanyLocaitonList_request, GetCompanyLocaitonList_response, GetCompanyLocationDetail_request, GetCompanyLocationDetail_response,
  ManageCompanyLocation_request, ManageCompanyLocation_response
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

  GetCompanyLocaitonList(model: GetCompanyLocaitonList_request): Observable<GetCompanyLocaitonList_response> {
    return this._HttpClient.post(AppCommon.APIURL + "/GetCompanyLocaitonList", model).map(x => <GetCompanyLocaitonList_response>x).catch(this.httperrorHandle);
  }

  ManageCompanyLocation(model: ManageCompanyLocation_request): Observable<ManageCompanyLocation_response> {
    return this._HttpClient.post(AppCommon.APIURL + "/ManageCompanyLocation", model).map(x => <ManageCompanyLocation_response>x).catch(this.httperrorHandle);
  }

  GetCompanyLocationDetail(model: GetCompanyLocationDetail_request): Observable<GetCompanyLocationDetail_response> {
    return this._HttpClient.post(AppCommon.APIURL + "/GetCompanyLocationDetail", model).map(x => <GetCompanyLocationDetail_response>x).catch(this.httperrorHandle);
  }

  ActiveInActiveCompanyLocation(model: GetCompanyLocationDetail_request): Observable<AppCommonResponse> {
    return this._HttpClient.post(AppCommon.APIURL + "/ActiveInActiveCompanyLocation", model).map(x => <AppCommonResponse>x).catch(this.httperrorHandle);
  }

    httperrorHandle(error: HttpErrorResponse) {
        return Observable.throw(error);
    }
}
