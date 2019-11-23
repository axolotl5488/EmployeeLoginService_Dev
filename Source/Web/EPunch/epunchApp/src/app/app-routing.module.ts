import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { AppRoutingServiceService } from './AppService/app-routing-service.service';
import { EmployeePunchComponent } from './AppComponent/employee-punch/employee-punch.component';

const routes: Routes = [
    { path: 'employeepunch', component: EmployeePunchComponent, canActivate: [AppRoutingServiceService], data: ['employeepunch'] },
    { path: '', redirectTo: '/employeepunch', pathMatch: 'full', canActivate: [AppRoutingServiceService], data: ['employeepunch'] },
    { path: '**', redirectTo: '/employeepunch', canActivate: [AppRoutingServiceService], data: ['employeepunch'] }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule],
    providers: [AppRoutingServiceService]
})
export class AppRoutingModule { }
