import { Component, OnInit } from '@angular/core';
import { ResultStatus } from '../../AppModel/appmodel_Model';
import { User_Service } from '../../AppService/User_Service';
import { LoginModel } from '../../AppModel/User_Models';
import { AppServiceService } from '../../AppService/app-routing-service.service';
import { Router, ActivatedRoute } from '@angular/router';
import { AppCommon } from '../../AppCommon/AppCommon'
import * as $ from 'jquery';

@Component({
    selector: 'app-login',
    templateUrl: './login.component.html',
    providers: [User_Service]
})
export class AppLoginComponent implements OnInit {

    model: LoginModel;
    constructor(private _User_Service: User_Service, private _AppServiceService: AppServiceService, private _Router: Router, private _ActivatedRoute: ActivatedRoute) {
        this.model = new LoginModel();
    }

    ngOnInit() {
        
    }

    SignIn(): void {
        this._User_Service.Token(this.model).subscribe(x => {
            this._AppServiceService.SetToken(x.access_token);
            //this._Router.navigate(["/company"]);
            window.location.href = AppCommon.RedirectURL+ "/company";

        }, err => {
                $("#loading").hide();
                alert("invalid credential");
        });
    }
    
}

