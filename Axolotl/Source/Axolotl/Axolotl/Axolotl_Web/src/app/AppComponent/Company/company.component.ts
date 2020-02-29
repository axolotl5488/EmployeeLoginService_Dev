import { Component, OnInit } from '@angular/core';
import { ResultStatus } from '../../AppModel/appmodel_Model';
import { Company_Service } from '../../AppService/Company_Service';
import { GetCompany_Detail, GetCompany_Response } from '../../AppModel/Company_Models'
import * as $ from 'jquery';

@Component({
    selector: 'app-company',
    templateUrl: './company.component.html',
    providers: [Company_Service]
})
export class AppCompanyComponent implements OnInit {

    model: GetCompany_Response;
    constructor(private _Company_Service: Company_Service) {
        this.model = new GetCompany_Response();
    }

    ngOnInit() {
        this.GetCompanyList();
    }

    GetCompanyList(): void {
        this._Company_Service.GetCompanyList().subscribe(x => {
            this.model = x;
        })
    }
}
