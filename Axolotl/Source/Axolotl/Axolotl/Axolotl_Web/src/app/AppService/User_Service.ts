import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/Observable/throw';
import { Observable } from 'rxjs/Observable';
import { ResultStatus, AppCommonResponse } from '../AppModel/appmodel_Model'
import { AppCommon } from '../AppCommon/AppCommon'
import {
  GetUserList_Response, GetUserList_Detail, SignUp_request, SignUp_response,
  GetUserDetail_Request, GetUserDetail_Response, ReportingPerson_request, GetEmployeeWeekOffs_detail, GetEmployeeWeekOffs_request, GetEmployeeWeekOffs_response
} from '../AppModel/User_Models';

import {
  EmployeeleaveList_Response
} from '../AppModel/EmployeeLeave_Model'

import {
  EmployeePunchList_Response
} from '../AppModel/EmployePunch_Model'


@Injectable()
export class User_Service {
  constructor(private _HttpClient: HttpClient) { }

  GetUserList(): Observable<GetUserList_Response> {
    return this._HttpClient.post(AppCommon.APIURL + "/GetUserList", null).map(x => <GetUserList_Response>x).catch(this.httperrorHandle);
  }

  SignUp(model: SignUp_request): Observable<SignUp_response> {
    
    return this._HttpClient.post(AppCommon.APIURL + "/SignUp", model).map(x => <SignUp_response>x).catch(this.httperrorHandle);
  }

  GetUserDetail(model: GetUserDetail_Request): Observable<GetUserDetail_Response> {
    return this._HttpClient.post(AppCommon.APIURL + "/GetUserDetail", model).map(x => <GetUserDetail_Response>x).catch(this.httperrorHandle);
  }

  UpdateUserDetail(model: SignUp_request): Observable<ResultStatus> {
    return this._HttpClient.post(AppCommon.APIURL + "/UpdateUserDetail", model).map(x => <ResultStatus>x).catch(this.httperrorHandle);
  }

  GetMyteamEmployeeList(model: ReportingPerson_request): Observable<GetUserList_Response> {
    return this._HttpClient.post(AppCommon.APIURL + "/GetMyteamEmployeeList", model).map(x => <GetUserList_Response>x).catch(this.httperrorHandle);
  }

  GetMyteamEmployeePunchList(model: ReportingPerson_request): Observable<EmployeePunchList_Response> {
    return this._HttpClient.post(AppCommon.APIURL + "/GetMyteamEmployeePunchList", model).map(x => <EmployeePunchList_Response>x).catch(this.httperrorHandle);
  }

  GetMyTeamEmployeeLeaveList(model: ReportingPerson_request): Observable<EmployeeleaveList_Response> {
    return this._HttpClient.post(AppCommon.APIURL + "/GetMyTeamEmployeeLeaveList", model).map(x => <EmployeeleaveList_Response>x).catch(this.httperrorHandle);
  }

  ManageEmployeeWeekOffs(model: GetEmployeeWeekOffs_detail): Observable<AppCommonResponse> {
    return this._HttpClient.post(AppCommon.APIURL + "/ManageEmployeeWeekOffs", model).map(x => <AppCommonResponse>x).catch(this.httperrorHandle);
  }

  GetEmployeeWeekOffs(model: GetEmployeeWeekOffs_request): Observable<GetEmployeeWeekOffs_response> {
    return this._HttpClient.post(AppCommon.APIURL + "/GetEmployeeWeekOffs", model).map(x => <GetEmployeeWeekOffs_response>x).catch(this.httperrorHandle);
  }

  httperrorHandle(error: HttpErrorResponse) {
    return Observable.throw(error);
  }
}
