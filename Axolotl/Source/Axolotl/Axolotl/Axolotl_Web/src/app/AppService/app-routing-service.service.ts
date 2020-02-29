import { AppModel } from '../AppModel/appmodel_Model'
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/Observable/throw';
import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, CanLoad } from '@angular/router';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpInterceptor, HttpHandler, HttpRequest, HttpEvent, HttpResponse, HttpHeaders, HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import { promise } from 'protractor';
import { error, debug } from 'util';
import { AppCommon } from '../AppCommon/AppCommon';
declare var $: any;





@Injectable()
export class AppServiceService {
    constructor(private _HttpClient: HttpClient) { }
    SetToken(token: string): void {
        localStorage.setItem('axolotl-web-token', token);
    }

    GetToken(): void {
        AppCommon.token = localStorage.getItem("axolotl-web-token");

    }

    RemoveToken(): boolean {
        localStorage.removeItem("axolotl-web-token");
        return true;
    }

    IsUserLogin(): boolean {

        this.GetToken();
        if ((AppCommon.token == null || AppCommon.token == undefined || AppCommon.token == ''))
        {
            return false;
        }
        else {
            return true;
        }
    }

    httperrorHandle(error: HttpErrorResponse) {
        return Observable.throw(error);
    }

    GetActivTab(): string {
        return AppCommon.activetab;
    }
}

@Injectable()
export class Authorized implements CanActivate {
    constructor(private _Authentication_Service: AppServiceService, private _Router: Router) {
    }
    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
        AppCommon.activetab = route.data[0];
        if (this._Authentication_Service.IsUserLogin()) {
            return true;
        } else {
            this._Router.navigate(['/login']);
            return false;
        }
    }
}

@Injectable()
export class UnAuthorized implements CanActivate {
    constructor(private _Authentication_Service: AppServiceService, private _Router: Router) {
    }
    
    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
        this._Authentication_Service.RemoveToken();
        console.log(this._Authentication_Service.IsUserLogin());
        if (this._Authentication_Service.IsUserLogin()) {
            this._Router.navigate(['/login']);
            return false;
        } else {
            return true;
        }
    }
}





//@Injectable()
//export class AppRoutingServiceService implements CanActivate {

//    appMmodel: AppModel;
//    constructor() {
//        this.appMmodel = new AppModel();
//    }
//    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
//        this.appMmodel.activeTab = route.data[0];
//        return true;
//    }
//}
