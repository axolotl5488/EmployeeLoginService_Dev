import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, CanLoad } from '@angular/router';
import { AppModel } from '../AppModel/appmodel_Model'


@Injectable()
export class AppRoutingServiceService implements CanActivate {

    appMmodel: AppModel;
    constructor() {
        this.appMmodel = new AppModel();
    }
    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
        this.appMmodel.activeTab = route.data[0];
        return true;
    }
}
