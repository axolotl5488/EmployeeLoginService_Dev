import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { AppRoutingServiceService } from './AppService/app-routing-service.service';
import { EmployeePunchComponent } from './AppComponent/employee-punch/employee-punch.component';
import { AppCompanyComponent } from './AppComponent/Company/company.component';
import { AppUserComponent } from './AppComponent/User/user.component';
import { AppEmployeePunchComponent } from './AppComponent/EmployePunch/employeepunch.component';
import { AppManageCompanyComponent } from './AppComponent/ManageCompany/managecompany.component';
import { AppManageUserComponent } from './AppComponent/ManageUser/manageuser.component';

const routes: Routes = [
  { path: 'manageuser/:id', component: AppManageUserComponent, canActivate: [AppRoutingServiceService], data: ['user'] }, 
    { path: 'user', component: AppUserComponent, canActivate: [AppRoutingServiceService], data: ['user'] },
    { path: 'managecompany/:id', component: AppManageCompanyComponent, canActivate: [AppRoutingServiceService], data: ['company'] },
    { path: 'company', component: AppCompanyComponent, canActivate: [AppRoutingServiceService], data: ['company'] },
    { path: 'employeepunch', component: AppEmployeePunchComponent, canActivate: [AppRoutingServiceService], data: ['employeepunch'] },
    { path: '', redirectTo: '/company', pathMatch: 'full', canActivate: [AppRoutingServiceService], data: ['company'] },
    { path: '**', redirectTo: '/company', canActivate: [AppRoutingServiceService], data: ['company'] }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule],
    providers: [AppRoutingServiceService]
})
export class AppRoutingModule { }
