import { Component, OnInit } from '@angular/core';
import { ResultStatus } from '../../AppModel/appmodel_Model';
import { Router, ActivatedRoute } from '@angular/router';
import { AppCommon } from '../../AppCommon/AppCommon';
import { User_Service } from '../../AppService/User_Service';
import { Global_Service } from '../../AppService/Global_Service';
import {
  SignUp_request, SignUp_response, GetUserDetail_Request, GetUserDetail_Response,
  GetUserList_Detail, GetUserList_Response
} from '../../AppModel/User_Models';
import { GetCompanyList_drp_response } from '../../AppModel/Global_Models'


@Component({
  selector: 'app-manageuser',
  templateUrl: './manageuser.component.html',
  providers: [User_Service]
})
export class AppManageUserComponent implements OnInit {

  model: SignUp_request;
  company_list: GetCompanyList_drp_response;
  activeTab: number;
  constructor(private _Router: Router, private _ActivatedRoute: ActivatedRoute,private _User_Service: User_Service, private _Global_Service: Global_Service) {
    this.model = new SignUp_request();
    this.company_list = new GetCompanyList_drp_response();
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
  }

}

