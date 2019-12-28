import { Component, OnInit } from '@angular/core';
import { ResultStatus } from '../../AppModel/appmodel_Model';
import { EmployeeLeave_Service } from '../../AppService/EmployeeLeave_Service';
import {
    EmployeeleaveList_Detail, EmployeeleaveList_Response,EmployeeleaveList_request
} from '../../AppModel/EmployeeLeave_Model'

import 'bootstrap/dist/js/bootstrap.js';
import 'bootstrap/dist/css/bootstrap.css';
import * as $ from 'jquery';


@Component({
    selector: 'app-employeeleave',
    templateUrl: './EmployeeLeaves.component.html',
    providers: [EmployeeLeave_Service]
})
export class AppEmployeeLeaveComponent implements OnInit {

    model: EmployeeleaveList_Response;
    constructor(private _EmployeeLeave_Service: EmployeeLeave_Service) {
        this.model = new EmployeeleaveList_Response();
    }

    ngOnInit() {
        this.EmployeeLeaveList();
    }

    EmployeeLeaveList(): void {
        let request: EmployeeleaveList_request = new EmployeeleaveList_request();
        request.companyid = 0;
        request.userid = 0;
        this._EmployeeLeave_Service.EmployeeLeaveList(request).subscribe(x => {
            this.model = x;
        })
    }
   
}
