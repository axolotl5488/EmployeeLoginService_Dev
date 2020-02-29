import { Component } from '@angular/core';
import { AppServiceService } from './AppService/app-routing-service.service';
import { AppCommon } from '../../src/app/AppCommon/AppCommon';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
    constructor(public _AppRoutingServiceService: AppServiceService) { }
    title = 'epunchApp';

    getsd() {
      //  this._AppRoutingServiceService.appMmodel
    }

    Logout(): void {
        this._AppRoutingServiceService.RemoveToken();
        window.location.href = AppCommon.RedirectURL +"/login";
    }
}
