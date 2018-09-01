import { Injectable } from '@angular/core';
import { LoginUser } from '../_models/login-user';
import { RegisterUser } from '../_models/register-user';
import { LoginUserResponse } from '../_models/login-user-response';
import { HttpClient, HttpResponse, HttpHeaders } from '@angular/common/http';
import { RegisterUserResponse } from '../_models/register-user-response';
import { CsrfResponse } from '../_models/csrf-response';
import { BehaviorSubject, timer, Observable } from 'rxjs';
import { LoginComponent } from '../../shared/login/login.component';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor(private http: HttpClient, private modalService: NgbModal, private router: Router) {}

  // auth refresh state
  private lastRefresh: number = Date.now();

  // auth login state
  loggedIn: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  loginName: BehaviorSubject<string> = new BehaviorSubject<string>("");
  role: BehaviorSubject<string> = new BehaviorSubject<string>("");

  async login(user: LoginUser): Promise<LoginUserResponse> {

    try {
      var headers: HttpHeaders = new HttpHeaders({ "Content-Type": "application/json" });
      var response: HttpResponse<LoginUserResponse> = await this.http.post<LoginUserResponse>("api/account/login", JSON.stringify(user), {
        headers: headers,
        observe: "response"
      }).toPromise();

      if (response.status == 200) {

        // update CSRF token
        await this.updateCsrfToken();

        this.loggedIn.next(true);
        this.loginName.next(response.body.name);
        this.role.next(response.body.role);

        return await new LoginUserResponse("ok", response.body.name, response.body.role);

      } else {
        return await new LoginUserResponse("fail");

      }
    } catch {
      return await new LoginUserResponse("fail");
    }
    
  }

  async logout(): Promise<boolean> {
    try {
      var headers: HttpHeaders = new HttpHeaders({ "Content-Type": "application/json" });
      var response: HttpResponse<null> = await this.http.post<null>("api/account/logout", "", {
        headers: headers,
        observe: "response"

      }).toPromise();

      if (response.status == 200) {
        // update CSRF token
        await this.updateCsrfToken();

        this.loggedIn.next(false);
        this.loginName.next("");
        this.role.next("");

        return await true;

      } else {
        return await false;

      }
    } catch {
      return await false;
    }
  }

  async register(user: RegisterUser): Promise<RegisterUserResponse> {
    try {
      var response: HttpResponse<RegisterUserResponse> = await this.http.post<RegisterUserResponse>("api/account/register", JSON.stringify(user), {
        observe: "response",
        headers: {
          "Content-Type": "application/json"
        }
      }).toPromise();
      
      if (response.status == 200) {
        return await new RegisterUserResponse("ok");

      } else {
        return await new RegisterUserResponse("fail");
      }

    } catch (e) {
      return new RegisterUserResponse("fail", e.error);
    }
  }

  async updateCsrfToken(): Promise<HttpResponse<CsrfResponse>> {

    var response: HttpResponse<CsrfResponse> = await this.http.get<CsrfResponse>("api/account/getCSRFToken", {
      observe: "response"
    }).toPromise();

    return response;
  }

  async refreshAuth(forced?: boolean): Promise<LoginUserResponse> {
    try {
      // check auth and set as 'logged out' if request made after 10 minutes of inactivity
      if (this.lastRefresh + 600000 < Date.now()) {
        this.lastRefresh = Date.now();

        var isAuth = await this.isAuth();

        if (!isAuth) {
          this.loggedIn.next(false);
          this.loginName.next("");
          this.role.next("");
          await this.updateCsrfToken();

        }

        return new LoginUserResponse("fail");
      }

      // ignore if request made within 1 minutes of previous request
      if (this.lastRefresh + 60000 > Date.now() && !forced)
        return new LoginUserResponse("ok", this.loginName.value, this.role.value);

      // update last refresh
      this.lastRefresh = Date.now();

      var response: HttpResponse<LoginUserResponse> = await this.http.post<LoginUserResponse>("api/account/refreshAuth", null, {
        observe: "response"
      }).toPromise();
      
      if (response.status == 200) {

        // update CSRF token
        await this.updateCsrfToken();

        this.loggedIn.next(true);
        this.loginName.next(response.body.name);
        this.role.next(response.body.role);

        return await new LoginUserResponse("ok", response.body.name, response.body.role);

      } else {
        return await new LoginUserResponse("fail");

      }
    } catch {
      return await new LoginUserResponse("fail");
    }

  }

  async isAuth(): Promise<boolean> {
    try {
      var response: HttpResponse<boolean> = await this.http.get<boolean>("api/account/isAuth", {
        observe: "response",
      }).toPromise();
      return response.status == 200 && response.body == true;
    } catch {
      return false;
    }
  }

}
