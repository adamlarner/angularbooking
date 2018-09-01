import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from '../shared/login/login.component';
import { RegistrationComponent } from './registration/registration.component';
import { RegistrationCompleteComponent } from './registration/registration-complete.component';
import { AuthGuardService } from './_services/auth-guard.service';
import { EmailConfirmedComponent } from './registration/email-confirmed.component';

const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'site'
  },
  {
    path: 'admin',
    loadChildren: '../admin/admin.module#AdminModule',
    canActivate: [AuthGuardService],
    data: { state: 'admin' }
  },
  {
    path: 'site',
    loadChildren: '../site/site.module#SiteModule',
    data: { state: 'site' }
  },
  {
    path: 'booking',
    loadChildren: '../booking/booking.module#BookingModule'
  },
  {
    path: 'register',
    component: RegistrationComponent,
    data: { state: 'registration' }
  },
  {
    path: 'registration-complete',
    component: RegistrationCompleteComponent,
    data: { state: 'registrationComplete' }
  },
  {
    path: 'email-confirmed',
    component: EmailConfirmedComponent,
    data: { state: 'emailConfirmed' }
  }
]

@NgModule({
  imports: [
    RouterModule.forRoot(routes)
  ],
  exports: [
    RouterModule
  ],
  declarations: []
})
export class CoreRoutingModule { }
