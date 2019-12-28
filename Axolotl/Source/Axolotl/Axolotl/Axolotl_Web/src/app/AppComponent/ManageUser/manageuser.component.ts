import { Component, OnInit } from '@angular/core';
import { ResultStatus } from '../../AppModel/appmodel_Model';
import { Router, ActivatedRoute } from '@angular/router';
import { AppCommon } from '../../AppCommon/AppCommon';
import { User_Service } from '../../AppService/User_Service';
import { EmployeePunch_Service } from '../../AppService/EmployePunch_Service';
import { EmployeeLeave_Service } from '../../AppService/EmployeeLeave_Service';
import { Global_Service } from '../../AppService/Global_Service';
import {
    SignUp_request, SignUp_response, GetUserDetail_Request, GetUserDetail_Response,
    GetUserList_Detail, GetUserList_Response
} from '../../AppModel/User_Models';

import {
    EmployeePunchList_Response, EmployeePunchList_Request
} from '../../AppModel/EmployePunch_Model'

import {
    EmployeeleaveList_Detail, EmployeeleaveList_Response, EmployeeleaveList_request
} from '../../AppModel/EmployeeLeave_Model'

import { GetCompanyList_drp_response } from '../../AppModel/Global_Models'


@Component({
    selector: 'app-manageuser',
    templateUrl: './manageuser.component.html',
    providers: [User_Service, EmployeePunch_Service, EmployeeLeave_Service]
})
export class AppManageUserComponent implements OnInit {

    model: SignUp_request;
    company_list: GetCompanyList_drp_response;
    model_employeepunchelist: EmployeePunchList_Response;
    model_employeeleaveslist: EmployeeleaveList_Response;
    activeTab: number;
    constructor(private _Router: Router, private _ActivatedRoute: ActivatedRoute, private _User_Service: User_Service, private _Global_Service: Global_Service, private _EmployeePunch_Service: EmployeePunch_Service, private _EmployeeLeave_Service: EmployeeLeave_Service) {
        this.model = new SignUp_request();
        this.company_list = new GetCompanyList_drp_response();
        this.model_employeepunchelist = new EmployeePunchList_Response();
        this.model_employeeleaveslist = new EmployeeleaveList_Response();
        this.activeTab = 1;
    }

    ngOnInit() {
        this.model.id = parseInt(this._ActivatedRoute.snapshot.params["id"]);
        this.GetCompanyList_drp();
        if (this.model.id > 0) {

            let request: GetUserDetail_Request = new GetUserDetail_Request();
            request.id = this.model.id;
            this._User_Service.GetUserDetail(request).subscribe(x => {
                this.model = x.record;
            })
        }
        else {
            this.model.companyid = undefined;
            this.model.Shifttype = 0;
        }
    }

    GetCompanyList_drp(): void {
        this._Global_Service.GetCompanyList_drp().subscribe(x => {
            this.company_list = x;
        })
    }

    ManageUser(): void {
        if (this.model.id == 0) {
            this.model.username = this.model.phonenumber;
            this._User_Service.SignUp(this.model).subscribe(x => {
                if (x.result.status) {
                    AppCommon.SuccessNotify("record added successfully!")
                    this._Router.navigate(["/user"])
                }
                else {
                    AppCommon.DangerNotify(x.result.message)
                }
            })
        }
        else {
            this.model.username = this.model.phonenumber;

            this._User_Service.UpdateUserDetail(this.model).subscribe(x => {
                if (x.status) {
                    AppCommon.SuccessNotify("record updated successfully!")
                    this._Router.navigate(["/user"])
                }
                else {
                    AppCommon.DangerNotify(x.message)
                }
            })
        }
    }

    ChangeTab(tabid: number): void {

        this.activeTab = tabid;
           if (this.activeTab == 2) {
            this.EmployeePunchList();
        }
        else if (this.activeTab == 3) {
            this.EmployeeLeaveList();
        }
    }

    EmployeePunchList(): void {
        let request: EmployeePunchList_Request = new EmployeePunchList_Request();
        request.companyid = 0;
        request.userid = this.model.id;
        this._EmployeePunch_Service.EmployeePunchList(request).subscribe(x => {
            this.model_employeepunchelist = x;
        })
    }

    EmployeeLeaveList(): void {
        let request: EmployeeleaveList_request = new EmployeeleaveList_request();
        request.companyid = 0;
        request.userid = this.model.id;
        this._EmployeeLeave_Service.EmployeeLeaveList(request).subscribe(x => {
            this.model_employeeleaveslist = x;
        })
    }



}

