import { Component, OnInit,AfterViewInit } from '@angular/core';
import { ResultStatus } from '../../AppModel/appmodel_Model';
import { Router, ActivatedRoute } from '@angular/router';
import { EmployeePunch_Service } from '../../AppService/EmployePunch_Service';
import {
  EmployeePunchList_Response, EmployeePunchList_Detail, GetEmployeePunchDetailWeb_request, GetEmployeePunchDetailWeb_response, PunchTask_Model, EmployeePunchList_Request
} from '../../AppModel/EmployePunch_Model'

import 'bootstrap/dist/js/bootstrap.js';
import 'bootstrap/dist/css/bootstrap.css';
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
  selector: 'app-manageemployeepunch',
  templateUrl: './manageemployeepunch.component.html',
  providers: [EmployeePunch_Service]
})
export class AppManageEmployeePunchComponent implements OnInit {

  
  activeTab: number;
  punchdetail_model: GetEmployeePunchDetailWeb_response;
  constructor(private _Router: Router, private _ActivatedRoute: ActivatedRoute,private _EmployeePunch_Service: EmployeePunch_Service) {
    this.punchdetail_model = new GetEmployeePunchDetailWeb_response();
    this.punchdetail_model.record = new EmployeePunchList_Detail();
  }

  ngOnInit() {
    this.activeTab = 1;
    this.punchdetail_model.record.id = parseInt(this._ActivatedRoute.snapshot.params["id"]);
    if (this.punchdetail_model.record.id > 0) {
      this.GetEmployeePunchDetail(this.punchdetail_model.record.id);
    }
  }

  ngAfterViewInit() {

  }

  ChangeTab(tabid: number): void {
    this.activeTab = tabid;

    
  }

  GetEmployeePunchDetail(id: number): void {
    let request: GetEmployeePunchDetailWeb_request = new GetEmployeePunchDetailWeb_request();
    request.punchid = id;
    this._EmployeePunch_Service.GetEmployeePunchDetail(request).subscribe(x => {
      this.punchdetail_model = x;
      let IsClockOut = true;
      if (x.record.clockouttime == 'no record')
        IsClockOut = false;

     this.ShowLocationonMap(x.record.clockinlatitude, x.record.clockinlongitude, 15, x.record.clockoutlatitude, x.record.clockoutlongitude, IsClockOut);
    })
  }

  ShowLocationonMap(lat, lng, zoom, outlat, outlng, IsClockOut) {
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
        infowindow.setContent("<b>Clocked In: </b>" + results[1].formatted_address);
        infowindow.open(map, marker);
      } else {
      }
    });

    if (IsClockOut) {
      var myoutLatLng = { lat: parseFloat(outlat), lng: parseFloat(outlng) };
      var marker1 = new google.maps.Marker({
        position: myoutLatLng,
        map: map,
        icon: "http://maps.google.com/mapfiles/ms/icons/blue-dot.png"
      });

      var outgeocoder = new google.maps.Geocoder;
      var outinfowindow = new google.maps.InfoWindow;
      outgeocoder.geocode({ 'location': myoutLatLng }, function (results, status) {
        if (status === 'OK') {
          outinfowindow.setContent("<b>Clocked Out: </b>" + results[1].formatted_address);
          outinfowindow.open(map, marker1);
        } else {
        }
      });
    }
    $(window).resize(function () {
      google.maps.event.trigger(map, "resize");
    });
  }
  

}

