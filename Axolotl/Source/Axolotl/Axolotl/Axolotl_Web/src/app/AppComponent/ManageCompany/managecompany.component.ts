import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { ResultStatus } from '../../AppModel/appmodel_Model';
import { Company_Service } from '../../AppService/Company_Service';
import { AppCommon } from '../../AppCommon/AppCommon';
import {
    GetCompanyDetail_Request, GetCompanyDetail_Response, ManageCompany_Request,ManageCompany_Response
} from '../../AppModel/Company_Models'


@Component({
    selector: 'app-managecompany',
    templateUrl: './managecompany.component.html',
    providers: [Company_Service]
})
export class AppManageCompanyComponent implements OnInit {

    model: ManageCompany_Request;
    activeTab: number;
    constructor(private _Router: Router, private _ActivatedRoute: ActivatedRoute, private _Company_Service: Company_Service) {
        this.model = new ManageCompany_Request();
        this.activeTab = 1;
    }

    ngOnInit() {
     //   this.GetCompanyList();
        this.model.id = parseInt(this._ActivatedRoute.snapshot.params["id"]);
        if (this.model.id > 0) {
            let request: GetCompanyDetail_Request = new GetCompanyDetail_Request();
            request.id = this.model.id;
            this._Company_Service.GetCompanyDetail(request).subscribe(x => {
                if (x.result.status) {
                    this.model = x.record;
                }
                else {
                    AppCommon.DangerNotify(x.result.message);
                }
            })
        }
        else {
            this.model = new ManageCompany_Request();
        }
    }

    ManageCompany(): void {
        this._Company_Service.ManageCompany(this.model).subscribe(x => {
            if (x.result.status) {
                this._Router.navigate(["/company"])
                AppCommon.SuccessNotify("record updated successfully")
            }
            else {
                AppCommon.DangerNotify(x.result.message);
            }
        })
    }

    ChangeTab(tabid: number): void {

    }
    
}
