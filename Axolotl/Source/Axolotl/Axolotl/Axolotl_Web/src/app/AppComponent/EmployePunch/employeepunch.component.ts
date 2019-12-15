import { Component, OnInit } from '@angular/core';
import { ResultStatus } from '../../AppModel/appmodel_Model';
import { EmployeePunch_Service } from '../../AppService/EmployePunch_Service';
import { EmployeePunchList_Response } from '../../AppModel/EmployePunch_Model'


@Component({
    selector: 'app-employeepunch',
    templateUrl: './employeepunch.component.html',
    providers: [EmployeePunch_Service]
})
export class AppEmployeePunchComponent implements OnInit {

    model: EmployeePunchList_Response;
    constructor(private _EmployeePunch_Service: EmployeePunch_Service) {
        this.model = new EmployeePunchList_Response();
    }

    ngOnInit() {
        this.EmployeePunchList();
    }

    EmployeePunchList(): void {
        this._EmployeePunch_Service.EmployeePunchList().subscribe(x => {
            this.model = x;
        })
    }
}
