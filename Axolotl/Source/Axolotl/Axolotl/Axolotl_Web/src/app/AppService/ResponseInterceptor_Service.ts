import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpHandler, HttpRequest, HttpEvent, HttpResponse, HttpHeaders, HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import * as $ from 'jquery';

//https://www.c-sharpcorner.com/article/angular-5-http-client-interceptors/
@Injectable()
export class ResponseInterceptor implements HttpInterceptor {
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    //console.log("Before sending data")
    //console.log(req);
    $("#loading").show();

    return next.handle(req).map(resp => {
      if (resp instanceof HttpResponse) {
        //console.log('Response is ::');
        $("#loading").hide();
      }
      return resp;
    }).catch((resp: any) => {
      //$("#honestabeloader").removeClass('show');
      if (resp instanceof HttpErrorResponse) {
          if (resp.status == 401 || resp.status == 403  || resp.status == 400) {
              $("#loading").hide();
        }
      }
      return Observable.throw(resp);
    });


  }
}  
