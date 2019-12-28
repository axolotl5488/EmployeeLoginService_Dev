import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';


import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { AppCompanyComponent } from './AppComponent/Company/company.component';
import { AppUserComponent } from './AppComponent/User/user.component';
import { AppEmployeePunchComponent } from './AppComponent/EmployePunch/employeepunch.component';
import { AppManageCompanyComponent } from './AppComponent/ManageCompany/managecompany.component';
import { AppManageUserComponent } from './AppComponent/ManageUser/manageuser.component';
import { AppEmployeeLeaveComponent } from './AppComponent/EmployeeLeaves/EmployeeLeaves.component';


import { ResponseInterceptor } from './AppService/ResponseInterceptor_Service';
import { Company_Service } from './AppService/Company_Service';
import { User_Service } from './AppService/User_Service';
import { EmployeePunch_Service } from './AppService/EmployePunch_Service';
import { Global_Service } from './AppService/Global_Service';
import { EmployeeLeave_Service } from './AppService/EmployeeLeave_Service';

@NgModule({
  declarations: [
    AppComponent, AppCompanyComponent, AppUserComponent, AppEmployeePunchComponent, AppManageCompanyComponent, AppManageUserComponent, AppEmployeeLeaveComponent
  ],
  imports: [
      BrowserModule,
      FormsModule,
      AppRoutingModule,
      HttpModule,
      HttpClientModule,
  ],
    providers: [Company_Service, User_Service, EmployeePunch_Service, Global_Service, EmployeeLeave_Service,
    { provide: HTTP_INTERCEPTORS, useClass: ResponseInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
