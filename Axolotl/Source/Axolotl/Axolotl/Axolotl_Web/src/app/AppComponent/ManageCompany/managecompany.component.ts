import { Component, OnInit, AfterViewInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { ResultStatus, AppCommonResponse } from '../../AppModel/appmodel_Model';
import { Company_Service } from '../../AppService/Company_Service';
import { AppCommon } from '../../AppCommon/AppCommon';
import {
  GetCompany_Detail, GetCompany_Response, GetCompanyDetail_Request, GetCompanyDetail_Response, ManageCompany_Request, ManageCompany_Response,
  GetCompanyLocaitonList_Detail, GetCompanyLocaitonList_request, GetCompanyLocaitonList_response, GetCompanyLocationDetail_request, GetCompanyLocationDetail_response,
  ManageCompanyLocation_request, ManageCompanyLocation_response
} from '../../AppModel/Company_Models'


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
  selector: 'app-managecompany',
  templateUrl: './managecompany.component.html',
  providers: [Company_Service]
})
export class AppManageCompanyComponent implements OnInit {

  model: ManageCompany_Request;
  model_companylocationlist: GetCompanyLocaitonList_response;
  model_companylocationrequest: ManageCompanyLocation_request;
  activeTab: number;

  constructor(private _Router: Router, private _ActivatedRoute: ActivatedRoute, private _Company_Service: Company_Service) {
    this.model = new ManageCompany_Request();
    this.model_companylocationlist = new GetCompanyLocaitonList_response();
    this.model_companylocationrequest = new ManageCompanyLocation_request();
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

  ngAfterViewInit() {
    // this.initAutocomplete();
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
  }


  GetCompanyLocaitonList(): void {

    let request: GetCompanyLocaitonList_request = new GetCompanyLocaitonList_request();
    request.companyid = this.model.id;
    this._Company_Service.GetCompanyLocaitonList(request).subscribe(x => {
      this.model_companylocationlist = x;
    })
  }

  ManageCompanyLocation(): void {

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
          $("#companylocation_popmodal").modal('show');
          this.initAutocomplete();
          this.ShowLocationonMap(x.record.lat, x.record.lng,15);
        }
        else {
          AppCommon.DangerNotify(x.result.message);
        }
      })
    }
    else {
      this.model_companylocationrequest = new ManageCompanyLocation_request();
      this.model_companylocationrequest.companyid = this.model.id;
      $("#companylocation_popmodal").modal('show');
      this.initAutocomplete();
      this.ShowLocationonMap(21.7679, 78.8718,4);
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



  // Map Feature
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
    this.ShowLocationonMap(place.geometry.location.lat(), place.geometry.location.lng(),15);
    this.SetAddressvalues();
  }

  // Bias the autocomplete object to the user's geographical location,
  // as supplied by the browser's 'navigator.geolocation' object.
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

  ShowLocationonMap(lat, lng,zoom) {
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
  }
