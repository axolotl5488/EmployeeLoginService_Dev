import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import {  Authorized, UnAuthorized } from './AppService/app-routing-service.service';
import { AppCompanyComponent } from './AppComponent/Company/company.component';
import { AppUserComponent } from './AppComponent/User/user.component';
import { AppEmployeePunchComponent } from './AppComponent/EmployePunch/employeepunch.component';
import { AppManageCompanyComponent } from './AppComponent/ManageCompany/managecompany.component';
import { AppManageUserComponent } from './AppComponent/ManageUser/manageuser.component';
import { AppEmployeeLeaveComponent } from './AppComponent/EmployeeLeaves/EmployeeLeaves.component';
import { AppManageEmployeePunchComponent } from './AppComponent/ManageEmployeePunch/manageemployeepunch.component';
import { AppLoginComponent } from './AppComponent/Login/login.component';


const routes: Routes = [
    { path: 'manageemployeepunch/:id', component: AppManageEmployeePunchComponent, canActivate: [Authorized], data: ['employeepunch'] }, 
    { path: 'employeeleaves', component: AppEmployeeLeaveComponent, canActivate: [Authorized], data: ['employeeleaves'] },
    { path: 'manageuser/:id', component: AppManageUserComponent, canActivate: [Authorized], data: ['user'] }, 
    { path: 'user', component: AppUserComponent, canActivate: [Authorized], data: ['user'] },
    { path: 'managecompany/:id', component: AppManageCompanyComponent, canActivate: [Authorized], data: ['company'] },
    { path: 'company', component: AppCompanyComponent, canActivate: [Authorized], data: ['company'] },
    { path: 'employeepunch', component: AppEmployeePunchComponent, canActivate: [Authorized], data: ['employeepunch'] },
    { path: 'login', component: AppLoginComponent, canActivate: [UnAuthorized], data: ['login'] },
    { path: '', redirectTo: '/company', pathMatch: 'full', canActivate: [Authorized], data: ['company'] },
    { path: '**', redirectTo: '/company', canActivate: [Authorized], data: ['company'] }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule],
    providers: [Authorized, UnAuthorized]
})
export class AppRoutingModule { }
