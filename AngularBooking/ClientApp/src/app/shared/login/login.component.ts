import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../core/_services/auth.service';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { LoginUserResponse } from '../../core/_models/login-user-response';
import { FormGroup, FormControl, Validators } from '@angular/forms';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent implements OnInit {

  constructor(
    private activeModal: NgbActiveModal
  ) { }

  isTimeout: boolean = false;
  
  loginForm: FormGroup;

  ngOnInit() {
    this.loginForm = new FormGroup({
      "email": new FormControl("", [
        Validators.email,
      ]),
      "password": new FormControl("", [
        Validators.required
      ])
    })
  }

  // callback for login service (typically AuthService 'login')
  loginCallback: (email, password) => Promise<LoginUserResponse>;

  async login(): Promise<void> {

    //var loginResponse = await this.authService.login({ email: this.email, password: this.password });
    var loginResponse: LoginUserResponse = await this.loginCallback(this.loginForm.controls.email.value, this.loginForm.controls.password.value);
    //if (loginResponse.status == "ok") {
      if (this.activeModal != null)
        this.activeModal.close();
    //}
  }

}
