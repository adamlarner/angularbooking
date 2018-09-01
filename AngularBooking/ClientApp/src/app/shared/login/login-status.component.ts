import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { LoginComponent } from '../../shared/login/login.component';
import { AuthService } from '../../core/_services/auth.service';
import { Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { delay } from 'rxjs/operators';
import { ProfileComponent } from '../profile/profile.component';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/components/common/messageservice';

@Component({
  selector: 'app-login-status',
  templateUrl: './login-status.component.html',
  styleUrls: ['./login-status.component.css']
})
export class LoginStatusComponent implements OnInit {

  constructor(private authService: AuthService, private modalService: NgbModal, private router: Router, private messageService: MessageService) { }

  loggedIn: boolean = false;
  name: string = "";
  role: string = "";

  changeFlag: boolean = false;

  login() {
    const modalRef = this.modalService.open(LoginComponent, {
      centered: true
    });
    let componentInstance: LoginComponent = modalRef.componentInstance;
    componentInstance.loginCallback = async (email, password) => {
      try {
        var loginResponse = await this.authService.login({ email, password });

        if (loginResponse.status == "ok") {
          this.messageService.add({ severity: 'info', summary: `Welcome ${loginResponse.name}`, closable: true, life: 2500 });
        } else {
          this.messageService.add({ severity: 'error', summary: "Login Failed", detail: "Incorrect Password", closable: true, life: 2500 });
        }

        return loginResponse;

      } catch (e) {
        this.messageService.add({ severity: 'error', summary: "Login Failed", detail: "Error Contacting Service", closable: true, life: 2500 });
      }
      
    }

    let modalObj = (modalRef as any);
    let modalWindow = modalObj._windowCmptRef.instance;
    setTimeout(() => {
      modalWindow.windowClass = 'modal-show'
    }, 50);

    let closeFunc = modalObj._removeModalElements.bind(modalRef);
    modalObj._removeModalElements = () => {
      modalWindow.windowClass = '';
      setTimeout(closeFunc, 250);
    };

    return false;
  }

  profile() {
    // check whether user is logged in first
    if (!this.authService.loggedIn)
      return false;

    const modalRef = this.modalService.open(ProfileComponent, {
      centered: true,
      size: "lg"
    });
    let componentInstance: ProfileComponent = modalRef.componentInstance;

    let modalObj = (modalRef as any);
    let modalWindow = modalObj._windowCmptRef.instance;
    setTimeout(() => {
      modalWindow.windowClass = 'modal-show'
    }, 50);

    let closeFunc = modalObj._removeModalElements.bind(modalRef);
    modalObj._removeModalElements = () => {
      modalWindow.windowClass = '';
      setTimeout(closeFunc, 250);
    };

    return false;
  }

  logout() {
    this.authService.logout().then(result => {
      this.messageService.add({ severity: 'info', summary: "You are now logged out", closable: true, life: 2500 });
    }).catch(e => {
      this.messageService.add({ severity: 'error', summary: "Logout Failed", detail: "Error Contacting Service", closable: true, life: 2500 });
    });

  }

  register() {
    this.router.navigateByUrl("register");
    return false;
  }

  routeToAdmin() {
    this.router.navigateByUrl("admin");
    return false;
  }

  ngOnInit() {
    this.authService.loggedIn.subscribe(data => {
      this.loggedIn = data;
    });
    this.authService.loginName.subscribe(data => {
      this.name = data;
    });
    this.authService.role.subscribe(data => {
      this.role = data;
    });
  }

}
