import { Component, OnInit, AfterViewInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { ResultStatus, AppCommonResponse } from '../../AppModel/appmodel_Model';
import { Company_Service } from '../../AppService/Company_Service';
import { EmployeePunch_Service } from '../../AppService/EmployePunch_Service';
import { EmployeeLeave_Service } from '../../AppService/EmployeeLeave_Service';
import { AppCommon } from '../../AppCommon/AppCommon';
import {
  GetCompany_Detail, GetCompany_Response, GetCompanyDetail_Request, GetCompanyDetail_Response, ManageCompany_Request, ManageCompany_Response,
  GetCompanyLocaitonList_Detail, GetCompanyLocaitonList_request, GetCompanyLocaitonList_response, GetCompanyLocationDetail_request, GetCompanyLocationDetail_response,
  ManageCompanyLocation_request, ManageCompanyLocation_response, GetCompanyHolidayDetail_request, GetCompanyHolidayDetail_response, GetCompanyHolidayList_detail,
  GetCompanyHolidayList_request, GetCompanyHolidayList_response, GetCompanyHolidayList_yeardetail, MangeCompantHolidays_request,
  GetCompanyRoleDetail_request, GetCompanyRoleDetail_response, GetCompanyRolesList_detail, GetCompanyRolesList_request, GetCompanyRolesList_response,
  GetCompanyRolesPermissionList_request, GetCompanyRolesPermissionList_response, GetCompanyRolesPermissionList_RolePermission_detail, GetCompanyRolesPermissionList_Role_detail,
  ManageCompanyRolesPermission_request, ManageCompanyRoles_request, ManageTeamList_detail, ManageTeamList_reporting_person_detail, ManageTeamList_reporting_role_detail,
  ManageTeamList_request, ManageTeamList_response, ManageTeam_request

} from '../../AppModel/Company_Models'
import {
  EmployeePunchList_Response, EmployeePunchList_Request
} from '../../AppModel/EmployePunch_Model'

import {
  EmployeeleaveList_Detail, EmployeeleaveList_Response, EmployeeleaveList_request
} from '../../AppModel/EmployeeLeave_Model'


import 'bootstrap/dist/js/bootstrap.js';
import 'bootstrap/dist/css/bootstrap.css';
import 'bootstrap-datepicker/dist/js/bootstrap-datepicker.js';
import * as moment from 'moment/moment.js';
import * as $ from 'jquery';
declare const google: any;

var placeSearch, autocomplete;
var componentForm = {
  //street_number: 'short_name',
  //route: 'long_name',
  locality: 'long_name',
  administrative_area_level_1: 'short_name',
  country: 'long_name',
  postal_code: 'short_name'
};

@Component({
  selector: 'app-managecompany',
  templateUrl: './managecompany.component.html',
  providers: [Company_Service, EmployeePunch_Service, EmployeeLeave_Service]
})
export class AppManageCompanyComponent implements OnInit {

  model: ManageCompany_Request;
  model_companylocationlist: GetCompanyLocaitonList_response;
  model_companylocationrequest: ManageCompanyLocation_request;

  model_companyholidayslist: GetCompanyHolidayList_response;
  model_companyholidayrequest: MangeCompantHolidays_request;
  model_companyholidayrequestbyyear: GetCompanyHolidayList_yeardetail;

  model_employeepunchelist: EmployeePunchList_Response;
  model_employeeleaveslist: EmployeeleaveList_Response;

  model_rolelist: GetCompanyRolesList_response;
  model_rolepermissionlist: GetCompanyRolesPermissionList_response;
  model_companyrolerequest: ManageCompanyRoles_request;
  model_myteamlist: ManageTeamList_response;
  activeTab: number;
  activeholidayear: string;

    
  constructor(private _Router: Router, private _ActivatedRoute: ActivatedRoute, private _Company_Service: Company_Service, private _EmployeePunch_Service: EmployeePunch_Service, private _EmployeeLeave_Service: EmployeeLeave_Service) {
    this.model = new ManageCompany_Request();
    this.model_companylocationlist = new GetCompanyLocaitonList_response();
    this.model_companylocationrequest = new ManageCompanyLocation_request();
    this.model_companyholidayslist = new GetCompanyHolidayList_response();
    this.model_companyholidayrequest = new MangeCompantHolidays_request();
    this.model_companyholidayrequestbyyear = new GetCompanyHolidayList_yeardetail();
    this.model_employeepunchelist = new EmployeePunchList_Response();
    this.model_employeeleaveslist = new EmployeeleaveList_Response();
    this.model_rolelist = new GetCompanyRolesList_response();
    this.model_rolepermissionlist = new GetCompanyRolesPermissionList_response();
    this.model_companyrolerequest = new ManageCompanyRoles_request();
    this.model_myteamlist = new ManageTeamList_response();
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
      this.model.punchrangeinmeter = 100;
    }
  }

  

    ngAfterViewInit(): void {
       
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

    this.activeTab = tabid;
    if (this.activeTab == 2) {
      this.GetCompanyLocaitonList();
    }
    else if (this.activeTab == 3) {
      this.GetCompanyHolidayList();
    }
    else if (this.activeTab == 4) {
      this.EmployeePunchList();
    }
    else if (this.activeTab == 5) {
      this.EmployeeLeaveList();
    }
    else if (this.activeTab == 6) {
      this.GetCompanyRolesList();
    }
    else if (this.activeTab == 7) {
      this.GetCompanyRolesPermissionList();
    }
    else if (this.activeTab == 8) {
      this.ManageTeamList();
    }

  }

  GetCompanyLocaitonList(): void {

    let request: GetCompanyLocaitonList_request = new GetCompanyLocaitonList_request();
    request.companyid = this.model.id;
    this._Company_Service.GetCompanyLocaitonList(request).subscribe(x => {
      this.model_companylocationlist = x;
    })
  }

  ManageCompanyLocation(): void {

    this.model_companylocationrequest.companyid = this.model.id;
    this._Company_Service.ManageCompanyLocation(this.model_companylocationrequest).subscribe(x => {
      if (x.result.status) {
        $("#companylocation_popmodal").modal('hide')
        AppCommon.SuccessNotify("record updated successfully");
        this.GetCompanyLocaitonList();
      }
      else {
        AppCommon.DangerNotify(x.result.message);
      }
    })
  }

  GetCompanyLocationDetail(id: number): void {

    if (id > 0) {
      let request: GetCompanyLocationDetail_request = new GetCompanyLocationDetail_request();
      request.locationid = id;
      this._Company_Service.GetCompanyLocationDetail(request).subscribe(x => {
        if (x.result.status) {
            this.model_companylocationrequest = x.record;
            if (this.model_companylocationrequest.imageurl == null || this.model_companylocationrequest.imageurl == undefined || this.model_companylocationrequest.imageurl == '') {
                this.model_companylocationrequest.imageurl = "assets/images/image_placeholder.png";
            }
          $("#companylocation_popmodal").modal('show');
          this.initAutocomplete();
          this.ShowLocationonMap(x.record.lat, x.record.lng, 15);
        }
        else {
          AppCommon.DangerNotify(x.result.message);
        }
      })
    }
    else {
      this.model_companylocationrequest = new ManageCompanyLocation_request();
        this.model_companylocationrequest.companyid = this.model.id;
        this.model_companylocationrequest.imageurl = "assets/images/image_placeholder.png";
      $("#companylocation_popmodal").modal('show');
      this.initAutocomplete();
      this.ShowLocationonMap(21.7679, 78.8718, 4);
    }
    }


    Image1Browse(): void {
        $("#Image1Browse").trigger("click")
    }

    Image1Upload(event): void {
        let uploadfile: FormData = new FormData();
        let files = event.target.files;
        if (files) {
            for (let file of files) {
                uploadfile.append("supplyitem", file);
                this._Company_Service.UploadImage(uploadfile).subscribe(x => {
                    if (x.response.status) {
                        this.model_companylocationrequest.imageurl = AppCommon.APIImageURL + "/AxololtImages/" + x.filename;
                    }
                })
            }
        }
    }

  ActiveInActiveCompanyLocation(id: number): void {

    let request: GetCompanyLocationDetail_request = new GetCompanyLocationDetail_request();
    request.locationid = id;
    this._Company_Service.ActiveInActiveCompanyLocation(request).subscribe(x => {
      if (x.result.status) {
        AppCommon.SuccessNotify("record updated successfully");
        this.GetCompanyLocaitonList();
      }
      else {
        AppCommon.DangerNotify(x.result.message);
      }
    })
  }

  initAutocomplete() {
    // Create the autocomplete object, restricting the search predictions to
    // geographical location types.
    autocomplete = new google.maps.places.Autocomplete(
      document.getElementById('autocomplete'), { componentRestrictions: { country: "IN" } });

    // Avoid paying for data that you don't need by restricting the set of
    // place fields that are returned to just the address components.
    // autocomplete.setFields(['address_component']);

    // When the user selects an address from the drop-down, populate the
    // address fields in the form.
    autocomplete.addListener('place_changed', this.fillInAddress);
  }

  fillInAddress = () => {
    // Get the place details from the autocomplete object.
    var place = autocomplete.getPlace();

    for (var i = 0; i < place.address_components.length; i++) {
      var addressType = place.address_components[i].types[0];
      if (componentForm[addressType]) {
        var val = place.address_components[i][componentForm[addressType]];
        if (addressType == "locality") {
          $("#locality").val(val);
        }
        if (addressType == "postal_code") {
          $("#postal_code").val(val);

        }
        if (addressType == "administrative_area_level_1") {
          $("#administrative_area_level_1").val(val);
        }
      }
    }
    $("#latitude").val(place.geometry.location.lat());
    $("#longitude").val(place.geometry.location.lng());
    this.ShowLocationonMap(place.geometry.location.lat(), place.geometry.location.lng(), 15);
    this.SetAddressvalues();
  }

  geolocate() {
    if (navigator.geolocation) {
      navigator.geolocation.getCurrentPosition(function (position) {
        var geolocation = {
          lat: position.coords.latitude,
          lng: position.coords.longitude
        };
        //var circle = new google.maps.Circle(
        //  { center: geolocation, radius: position.coords.accuracy });
        //autocomplete.setBounds(circle.getBounds());
      });
    }
  }

  ShowLocationonMap(lat, lng, zoom) {
    var myLatLng = { lat: parseFloat(lat), lng: parseFloat(lng) };
    var map = new google.maps.Map(document.getElementById('map'), {
      center: myLatLng,
      zoom: zoom,
      mapTypeId: google.maps.MapTypeId.ROADMAP,
    });
    var marker = new google.maps.Marker({
      position: myLatLng,
      map: map,
    });
    // Get Address from Lat/Lan
    var geocoder = new google.maps.Geocoder;
    var infowindow = new google.maps.InfoWindow;
    geocoder.geocode({ 'location': myLatLng }, function (results, status) {
      if (status === 'OK') {
        infowindow.setContent(results[1].formatted_address);
        infowindow.open(map, marker);
      } else {
      }
    });
    $(window).resize(function () {
      google.maps.event.trigger(map, "resize");
    });
  }

  SetAddressvalues() {

    this.model_companylocationrequest.address = $("#autocomplete").val();
    this.model_companylocationrequest.city = $("#locality").val();
    this.model_companylocationrequest.state = $("#administrative_area_level_1").val();
    this.model_companylocationrequest.zipcode = $("#postal_code").val();
    this.model_companylocationrequest.lat = $("#latitude").val();
    this.model_companylocationrequest.lng = $("#longitude").val();
  }

  // Company Holidays Start

  GetCompanyHolidayList(): void {

    let request: GetCompanyHolidayList_request = new GetCompanyHolidayList_request();
    request.companyid = this.model.id;
    this._Company_Service.GetCompanyHolidayList(request).subscribe(x => {
      this.model_companyholidayslist = x;
      if (x.records.length > 0) {
        this.HolidayChangeTab(x.records[0].year);
      }
    })
  }

  ManageCompanyHolidays(): void {
    this.model_companyholidayrequest.companyid = this.model.id;
    this._Company_Service.ManageCompanyHolidays(this.model_companyholidayrequest).subscribe(x => {
      if (x.result.status) {
        $("#companyholiday_popmodal").modal('hide')
        AppCommon.SuccessNotify("record updated successfully");
        this.GetCompanyHolidayList();
      }
      else {
        AppCommon.DangerNotify(x.result.message);
      }
    })
  }


  GetCompanyHolidayDetail(id: number): void {

    if (id > 0) {
      let request: GetCompanyHolidayDetail_request = new GetCompanyHolidayDetail_request();
      request.id = id;
      this._Company_Service.GetCompanyHolidayDetail(request).subscribe(x => {
        if (x.result.status) {
          this.model_companyholidayrequest = x.record;
            $("#companyholiday_popmodal").modal('show');

            let _this = this;
            $('#holidaydate').datepicker({
                format: 'dd/mm/yyyy',
            }).on('changeDate', function (e) {
                _this.model_companyholidayrequest.date = $("#holidaydate").val();
            });
        }
        else {
          AppCommon.DangerNotify(x.result.message);
        }
      })
    }
    else {
      this.model_companyholidayrequest = new MangeCompantHolidays_request();
      this.model_companylocationrequest.companyid = this.model.id;
        $("#companyholiday_popmodal").modal('show');

        let _this = this;
        $('#holidaydate').datepicker({
            format: 'dd/mm/yyyy',
        }).on('changeDate', function (e) {
            _this.model_companyholidayrequest.date = $("#holidaydate").val();
        });
    }
  }

  ActiveInActiveCompanyHolidays(id: number): void {

    let request: GetCompanyHolidayDetail_request = new GetCompanyHolidayDetail_request();
    request.id = id;
    this._Company_Service.ActiveInActiveCompanyHolidays(request).subscribe(x => {
      if (x.result.status) {
        AppCommon.SuccessNotify("record updated successfully");
        this.GetCompanyLocaitonList();
      }
      else {
        AppCommon.DangerNotify(x.result.message);
      }
    })
  }

  HolidayChangeTab(year: string): void {

    this.activeholidayear = year;
    let recordbyyear = this.model_companyholidayslist.records.filter(x => x.year == year)[0];
    this.model_companyholidayrequestbyyear = recordbyyear;
  }

  // Company Holidays End


  EmployeePunchList(): void {
    let request: EmployeePunchList_Request = new EmployeePunchList_Request();
    request.companyid = this.model.id;
    request.userid = 0;
    this._EmployeePunch_Service.EmployeePunchList(request).subscribe(x => {
      this.model_employeepunchelist = x;
    })
  }

  EmployeeLeaveList(): void {
    let request: EmployeeleaveList_request = new EmployeeleaveList_request();
    request.companyid = this.model.id;
    request.userid = 0;
    this._EmployeeLeave_Service.EmployeeLeaveList(request).subscribe(x => {
      this.model_employeeleaveslist = x;
    })
  }

  GetCompanyRolesList(): void {
    let request: GetCompanyRolesList_request = new GetCompanyRolesList_request();
    request.companyid = this.model.id;
    this._Company_Service.GetCompanyRolesList(request).subscribe(x => {
      this.model_rolelist = x;
    })
  }

  GetCompanyRolesPermissionList(): void {
    let request: GetCompanyRolesPermissionList_request = new GetCompanyRolesPermissionList_request();
    request.companyid = this.model.id;
    this._Company_Service.GetCompanyRolesPermissionList(request).subscribe(x => {
      this.model_rolepermissionlist = x;
    })
  }

  ShowManageCompanyRoles(id: number): void {

    if (id > 0) {
      let request: GetCompanyRoleDetail_request = new GetCompanyRoleDetail_request();
      request.id = id;
      this._Company_Service.GetCompanyRoleDetail(request).subscribe(x => {
        if (x.result.status) {
          this.model_companyrolerequest = x.record;
          $("#companyrole_popmodal").modal('show');
        }
        else {
          AppCommon.DangerNotify(x.result.message);
        }
      })
    }
    else {
      this.model_companyrolerequest = new ManageCompanyRoles_request();
      this.model_companyrolerequest.companyid = this.model.id;
      $("#companyrole_popmodal").modal('show');
    }
  }

  ManageCompanyRoles(): void {
    this._Company_Service.ManageCompanyRoles(this.model_companyrolerequest).subscribe(x => {
      if (x.result.status) {
        AppCommon.SuccessNotify("record updated successfully!");
        $("#companyrole_popmodal").modal('hide');
        this.GetCompanyRolesList();
      }
      else {
        AppCommon.DangerNotify(x.result.message);
      }
    })
  }

  ManageCompanyRolesPermission(id: number, roleid: number, screenid: number): void {

    let request: ManageCompanyRolesPermission_request = new ManageCompanyRolesPermission_request();
    request.companyid = this.model.id;
    request.companyroleid = roleid;
    request.companyrolepermissionid = id;
    request.screenid = screenid;
    this._Company_Service.ManageCompanyRolesPermission(request).subscribe(x => {
      if (x.result.status) {
        AppCommon.SuccessNotify("record updated successfully!");
        this.GetCompanyRolesPermissionList();
      }
      else {
        AppCommon.DangerNotify(x.result.message);
      }
    })
  }

  RemoveCompanyRoles(id: number): void {

    var r = confirm("Are you sure want to delete this role?, this will change all assigned this role");
    if (r == true) {
      let request: GetCompanyRoleDetail_request = new GetCompanyRoleDetail_request();
      request.id = id;
      this._Company_Service.RemoveCompanyRoles(request).subscribe(x => {
        if (x.result.status) {
          AppCommon.SuccessNotify("record deleted successfully!");
          this.GetCompanyRolesList();
        }
        else {
          AppCommon.DangerNotify(x.result.message);
        }
      })
    }
  }

  ManageTeamList(): void {
    let request: ManageTeamList_request = new ManageTeamList_request();
    request.companyid = this.model.id;
    this._Company_Service.ManageTeamList(request).subscribe(x => {
      this.model_myteamlist = x;
    })
  }

  ManageTeam(userid: number, roleid: number, reportingid?: number): void {
    let request: ManageTeam_request = new ManageTeam_request();
    request.userid = userid;
    request.roleid = roleid;
    request.reportingpersonid = reportingid;
    this._Company_Service.ManageTeam(request).subscribe(x => {
      if (x.result.status) {
        AppCommon.SuccessNotify("record updated successfully!");
        this.ManageTeamList();
      }
      else {
        AppCommon.DangerNotify(x.result.message);
      }
    })
  }

}
