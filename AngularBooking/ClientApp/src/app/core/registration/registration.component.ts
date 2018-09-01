import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators, ValidatorFn, AbstractControl } from '@angular/forms';
import { Router } from '@angular/router';
import { fadeAnimation } from '../_animations/fade-animation';
import { AuthService } from '../_services/auth.service';
import { MessageService } from 'primeng/components/common/messageservice';

@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.css'],
  animations: [fadeAnimation]
})
export class RegistrationComponent implements OnInit {

  constructor(private router: Router, private authService: AuthService, private messageService: MessageService) { }

  registrationFormGroup: FormGroup;

  async register() {
    let result = await this.authService.register({
      email: this.registrationFormGroup.controls.email.value,
      firstName: this.registrationFormGroup.controls.firstName.value,
      lastName: this.registrationFormGroup.controls.lastName.value,
      password: this.registrationFormGroup.controls.password.value,
      passwordConfirm: this.registrationFormGroup.controls.passwordConfirm.value
    });
    
    if (result.status == "ok") {
        this.router.navigateByUrl("registration-complete");
    } else {
      // todo: error messages for different errors
      if (result.error) {
        if (result.error.duplicateUserName) {
          this.messageService.add({ severity: "error", summary: "Registration error", detail: result.error.duplicateUserName[0], life: 4000 });
        } else {
          this.messageService.add({ severity: "error", summary: "Registration error", detail: "Unknown error", life: 4000 });
        }
        
      }
    }

      
  };

  cancel() {
    this.router.navigateByUrl("site/listing");
    return false;
  };

  ngOnInit() {
    this.registrationFormGroup = new FormGroup({
      email: new FormControl("", [
        Validators.required,
        Validators.email
      ]),
      firstName: new FormControl("", [
        Validators.required
      ]),
      lastName: new FormControl("", [
        Validators.required
      ]),
      password: new FormControl("", [
        Validators.required,
        Validators.minLength(8)
      ]),
      passwordConfirm: new FormControl("", [
        Validators.required,
        Validators.minLength(8)
      ])
    }, (control: AbstractControl) => {
      let password = control.get("password").value;
      let passwordConfirm = control.get("passwordConfirm").value;
      if (password !== passwordConfirm)
        return {
          invalid: true
        };
    });
  }

}
