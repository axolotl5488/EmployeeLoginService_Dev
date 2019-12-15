import { Component, OnInit } from '@angular/core';
import { ResultStatus } from '../../AppModel/appmodel_Model';
import { User_Service } from '../../AppService/User_Service';
import { GetUserList_Response } from '../../AppModel/User_Models'


@Component({
    selector: 'app-user',
    templateUrl: './user.component.html',
    providers: [User_Service]
})
export class AppUserComponent implements OnInit {

    model: GetUserList_Response;
    constructor(private _User_Service: User_Service) {
        this.model = new GetUserList_Response();
    }

    ngOnInit() {
        this.GetUserList();
    }

    GetUserList(): void {
        this._User_Service.GetUserList().subscribe(x => {
            this.model = x;
        })
    }
}

