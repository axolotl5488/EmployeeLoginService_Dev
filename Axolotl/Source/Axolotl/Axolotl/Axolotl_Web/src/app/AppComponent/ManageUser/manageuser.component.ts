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
  GetUserList_Detail, GetUserList_Response, ReportingPerson_request, GetEmployeeWeekOffs_detail, GetEmployeeWeekOffs_request, GetEmployeeWeekOffs_response
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
  model_myteam_employeepunchelist: EmployeePunchList_Response;
  model_myteam_employeelist: GetUserList_Response;
  model_myteam_employeeleaveslist: EmployeeleaveList_Response;
  model_weekoffs_list: GetEmployeeWeekOffs_response;
  activeTab: number;
  myteamactiveTab: number;
  constructor(private _Router: Router, private _ActivatedRoute: ActivatedRoute, private _User_Service: User_Service, private _Global_Service: Global_Service, private _EmployeePunch_Service: EmployeePunch_Service, private _EmployeeLeave_Service: EmployeeLeave_Service) {
    this.model = new SignUp_request();
    this.company_list = new GetCompanyList_drp_response();
    this.model_employeepunchelist = new EmployeePunchList_Response();
    this.model_employeeleaveslist = new EmployeeleaveList_Response();
    this.model_myteam_employeelist = new GetUserList_Response();
    this.model_myteam_employeepunchelist = new EmployeePunchList_Response();
    this.model_myteam_employeeleaveslist = new EmployeeleaveList_Response();
    this.model_weekoffs_list = new GetEmployeeWeekOffs_response();
    this.activeTab = 1;
    this.myteamactiveTab = 1;
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
      this.model.companyroleid = 2;//employee id
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
    else if (this.activeTab == 4) {
      this.myteamactiveTab = 1;
      this.GetMyteamEmployeeList();
    }
    else if (this.activeTab == 5) {
      this.GetEmployeeWeekOffs();
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



  MyTeamChangeTab(tabid: number): void {

    this.myteamactiveTab = tabid;
    if (this.myteamactiveTab == 1) {
      this.GetMyteamEmployeeList();
    }
    else if (this.myteamactiveTab == 2) {
      this.GetMyteamEmployeePunchList();
    }
    else if (this.myteamactiveTab == 3) {
      this.GetMyTeamEmployeeLeaveList();
    }
    
  }

  // My Team
  
  GetMyteamEmployeeList(): void {
    let request: ReportingPerson_request = new ReportingPerson_request();
    request.userid = this.model.id;
    request.companyid = this.model.companyid;
    this._User_Service.GetMyteamEmployeeList(request).subscribe(x => {
      this.model_myteam_employeelist = x;
    })
  }

  GetMyteamEmployeePunchList(): void {
    let request: ReportingPerson_request = new ReportingPerson_request();
    request.userid = this.model.id;
    request.companyid = this.model.companyid;
    this._User_Service.GetMyteamEmployeePunchList(request).subscribe(x => {
      this.model_myteam_employeepunchelist = x;
    })
  }

  GetMyTeamEmployeeLeaveList(): void {
    let request: ReportingPerson_request = new ReportingPerson_request();
    request.userid = this.model.id;
    request.companyid = this.model.companyid;
    this._User_Service.GetMyTeamEmployeeLeaveList(request).subscribe(x => {
      this.model_myteam_employeeleaveslist = x;
    })
  }

  GetEmployeeWeekOffs(): void {
    let request: GetEmployeeWeekOffs_request = new GetEmployeeWeekOffs_request();
    request.employeeid = this.model.id;
    this._User_Service.GetEmployeeWeekOffs(request).subscribe(x => {
      this.model_weekoffs_list = x;
    })
  }

  ManageEmployeeWeekOffs(request: GetEmployeeWeekOffs_detail): void {
    this._User_Service.ManageEmployeeWeekOffs(request).subscribe(x => {
      if (x.result.status) {
        AppCommon.SuccessNotify("record updated successfully!");
        
      }
      else {
        AppCommon.DangerNotify(x.result.message);
      }
      this.GetEmployeeWeekOffs();
    })
  }
}

