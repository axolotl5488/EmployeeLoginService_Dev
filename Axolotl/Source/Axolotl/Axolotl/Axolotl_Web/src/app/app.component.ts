import { Component } from '@angular/core';
import { AppRoutingServiceService } from './AppService/app-routing-service.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
    constructor(public _AppRoutingServiceService: AppRoutingServiceService) { }
    title = 'epunchApp';

    getsd() {
      //  this._AppRoutingServiceService.appMmodel
    }
}
