import { Component } from '@angular/core';
import { AppRoutingServiceService } from './AppService/app-routing-service.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
    constructor(private _AppRoutingServiceService: AppRoutingServiceService) { }
  title = 'epunchApp';
}
