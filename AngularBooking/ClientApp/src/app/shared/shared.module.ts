import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoginComponent } from './login/login.component';
import { AuthService } from '../core/_services/auth.service';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { LoginStatusComponent } from './login/login-status.component';
import { NgbModule, NgbPaginationConfig } from '@ng-bootstrap/ng-bootstrap';
import { HttpClientModule, HttpClientXsrfModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RefreshInterceptorService } from '../core/_services/refresh-interceptor.service';
import { ProfileComponent } from './profile/profile.component';
import { ToastModule } from 'primeng/toast';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    NgbModule,
    HttpClientModule,
    HttpClientXsrfModule,
    ToastModule
  ],
  providers: [{
    provide: HTTP_INTERCEPTORS,
    useClass: RefreshInterceptorService,
    multi: true
  }, NgbPaginationConfig],
  declarations: [LoginComponent, LoginStatusComponent, ProfileComponent],
  entryComponents: [LoginComponent, ProfileComponent],
  exports: [
    LoginComponent,
    LoginStatusComponent
  ]
})
export class SharedModule { }
