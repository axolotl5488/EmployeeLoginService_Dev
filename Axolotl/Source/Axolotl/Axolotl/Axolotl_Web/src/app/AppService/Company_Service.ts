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
  ManageCompanyLocation_request, ManageCompanyLocation_response, GetCompanyHolidayDetail_request, GetCompanyHolidayDetail_response, GetCompanyHolidayList_detail,
  GetCompanyHolidayList_request, GetCompanyHolidayList_response, GetCompanyHolidayList_yeardetail, MangeCompantHolidays_request,
  GetCompanyRolesList_response, GetCompanyRolesList_request, ManageCompanyRoles_request, GetCompanyRoleDetail_request, GetCompanyRoleDetail_response,
  GetCompanyRolesPermissionList_response, GetCompanyRolesPermissionList_request, ManageCompanyRolesPermission_request,
  GetCompanyRolesList_detail, GetCompanyRolesPermissionList_RolePermission_detail, GetCompanyRolesPermissionList_Role_detail, ManageTeamList_detail,
  ManageTeamList_reporting_person_detail, ManageTeamList_reporting_role_detail, ManageTeamList_request, ManageTeamList_response, ManageTeam_request
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

  ActiveInActiveCompanyLocation(model: GetCompanyLocationDetail_request): Observable<AppCommonResponse> {
    return this._HttpClient.post(AppCommon.APIURL + "/ActiveInActiveCompanyLocation", model).map(x => <AppCommonResponse>x).catch(this.httperrorHandle);
  }

  GetCompanyLocationDetail(model: GetCompanyLocationDetail_request): Observable<GetCompanyLocationDetail_response> {
    return this._HttpClient.post(AppCommon.APIURL + "/GetCompanyLocationDetail", model).map(x => <GetCompanyLocationDetail_response>x).catch(this.httperrorHandle);
  }

  ManageCompanyHolidays(model: MangeCompantHolidays_request): Observable<AppCommonResponse> {
    return this._HttpClient.post(AppCommon.APIURL + "/ManageCompanyHolidays", model).map(x => <AppCommonResponse>x).catch(this.httperrorHandle);
  }

  GetCompanyHolidayDetail(model: GetCompanyHolidayDetail_request): Observable<GetCompanyHolidayDetail_response> {
    return this._HttpClient.post(AppCommon.APIURL + "/GetCompanyHolidayDetail", model).map(x => <GetCompanyHolidayDetail_response>x).catch(this.httperrorHandle);
  }

  GetCompanyHolidayList(model: GetCompanyHolidayList_request): Observable<GetCompanyHolidayList_response> {
    return this._HttpClient.post(AppCommon.APIURL + "/GetCompanyHolidayList", model).map(x => <GetCompanyHolidayList_response>x).catch(this.httperrorHandle);
  }

  ActiveInActiveCompanyHolidays(model: GetCompanyHolidayDetail_request): Observable<AppCommonResponse> {
    return this._HttpClient.post(AppCommon.APIURL + "/ActiveInActiveCompanyHolidays", model).map(x => <AppCommonResponse>x).catch(this.httperrorHandle);
  }

  //----
  GetCompanyRolesList(model: GetCompanyRolesList_request): Observable<GetCompanyRolesList_response> {
    return this._HttpClient.post(AppCommon.APIURL + "/GetCompanyRolesList", model).map(x => <GetCompanyRolesList_response>x).catch(this.httperrorHandle);
  }

  ManageCompanyRoles(model: ManageCompanyRoles_request): Observable<AppCommonResponse> {
    return this._HttpClient.post(AppCommon.APIURL + "/ManageCompanyRoles", model).map(x => <AppCommonResponse>x).catch(this.httperrorHandle);
  }

  RemoveCompanyRoles(model: GetCompanyRoleDetail_request): Observable<AppCommonResponse> {
    return this._HttpClient.post(AppCommon.APIURL + "/RemoveCompanyRoles", model).map(x => <AppCommonResponse>x).catch(this.httperrorHandle);
  }

  GetCompanyRoleDetail(model: GetCompanyRoleDetail_request): Observable<GetCompanyRoleDetail_response> {
    return this._HttpClient.post(AppCommon.APIURL + "/GetCompanyRoleDetail", model).map(x => <GetCompanyRoleDetail_response>x).catch(this.httperrorHandle);
  }

  GetCompanyRolesPermissionList(model: GetCompanyRolesPermissionList_request): Observable<GetCompanyRolesPermissionList_response> {
    return this._HttpClient.post(AppCommon.APIURL + "/GetCompanyRolesPermissionList", model).map(x => <GetCompanyRolesPermissionList_response>x).catch(this.httperrorHandle);
  }

  ManageCompanyRolesPermission(model: ManageCompanyRolesPermission_request): Observable<AppCommonResponse> {
    return this._HttpClient.post(AppCommon.APIURL + "/ManageCompanyRolesPermission", model).map(x => <AppCommonResponse>x).catch(this.httperrorHandle);
  }

  ManageTeamList(model: ManageTeamList_request): Observable<ManageTeamList_response> {
    return this._HttpClient.post(AppCommon.APIURL + "/ManageTeamList", model).map(x => <ManageTeamList_response>x).catch(this.httperrorHandle);
  }

  ManageTeam(model: ManageTeam_request): Observable<AppCommonResponse> {
    return this._HttpClient.post(AppCommon.APIURL + "/ManageTeam", model).map(x => <AppCommonResponse>x).catch(this.httperrorHandle);
  }

  httperrorHandle(error: HttpErrorResponse) {
    return Observable.throw(error);
  }
}
