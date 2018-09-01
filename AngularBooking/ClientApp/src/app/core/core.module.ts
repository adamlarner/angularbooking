import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CoreRoutingModule } from './/core-routing.module';
import { RouterModule } from '@angular/router';
import { HttpClientModule, HttpClientXsrfModule } from '@angular/common/http';
import { AuthService } from './_services/auth.service';
import { SharedModule } from '../shared/shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RegistrationComponent } from './registration/registration.component';
import { RegistrationCompleteComponent } from './registration/registration-complete.component';
import { ToastModule } from 'primeng/toast';
import { EmailConfirmedComponent } from './registration/email-confirmed.component';

@NgModule({
  declarations: [RegistrationComponent, RegistrationCompleteComponent, EmailConfirmedComponent],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    CoreRoutingModule,
    HttpClientModule,
    HttpClientXsrfModule,
    SharedModule,
    ToastModule
  ],
  exports: [
    RouterModule
  ]

})
export class CoreModule { }
