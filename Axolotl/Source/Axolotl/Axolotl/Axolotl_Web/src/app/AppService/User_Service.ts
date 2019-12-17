import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/Observable/throw';
import { Observable } from 'rxjs/Observable';
import { ResultStatus } from '../AppModel/appmodel_Model'
import { AppCommon } from '../AppCommon/AppCommon'
import {
  GetUserList_Response, GetUserList_Detail, SignUp_request, SignUp_response
} from '../AppModel/User_Models'


@Injectable()
export class User_Service {
  constructor(private _HttpClient: HttpClient) { }

  GetUserList(): Observable<GetUserList_Response> {
    return this._HttpClient.post(AppCommon.APIURL + "/GetUserList", null).map(x => <GetUserList_Response>x).catch(this.httperrorHandle);
  }

  SignUp(model: SignUp_request): Observable<SignUp_response> {
    return this._HttpClient.post(AppCommon.APIURL + "/SignUp", model).map(x => <SignUp_response>x).catch(this.httperrorHandle);
  }

  httperrorHandle(error: HttpErrorResponse) {
    return Observable.throw(error);
  }
}
