import { TestBed, inject, fakeAsync } from '@angular/core/testing';
import { AuthService } from './auth.service';
import { HttpHandler, HttpClientModule, HttpResponse } from '@angular/common/http';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NgbModalStack } from '@ng-bootstrap/ng-bootstrap/modal/modal-stack';
import { RouterTestingModule } from '@angular/router/testing';
import { Router, RouterModule } from '@angular/router';
import { CsrfResponse } from '../_models/csrf-response';
import { LoginUserResponse } from '../_models/login-user-response';
import { Observable, of } from 'rxjs';

describe('AuthService', () => {

  let service: AuthService;
  let backend: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [
        HttpClientTestingModule,
        RouterTestingModule
      ],
      providers: [
        AuthService,
        NgbModal,
        NgbModalStack
      ]
    });

    // get mocked instances of SUT
    backend = TestBed.get(HttpTestingController);
    service = TestBed.get(AuthService);

  });

  it('should be created', (done) => {
    expect(service).toBeTruthy();
    done();
  });

  it('should attempt login and succeed', (done) => {
    var spy = spyOn(service, "updateCsrfToken").and.returnValue(new HttpResponse<CsrfResponse>());
    service.login({ email: "test@test.com", password: "Password.01" }).then(response => {

      expect(response.status).toEqual("ok");
      done();
    });

    const req = backend.expectOne("api/account/login");
    req.flush(new LoginUserResponse("ok", "user", "user"));

    backend.verify();
    
  });

  it('should attempt login and fail, login incorrect', (done) => {
    var spy = spyOn(service, "updateCsrfToken").and.returnValue(new HttpResponse<CsrfResponse>());
    service.login({ email: "test@test.com", password: "Password.01" }).then(response => {
      expect(response.status).toEqual("fail");
      done();
    });

    const req = backend.expectOne("api/account/login");
    req.flush(new LoginUserResponse(status = "fail"), { status: 404, statusText: "fail" });

    backend.verify();
  });

  it('should attempt to register new user and succeed', () => {
    service.register({ firstName: "Test", lastName: "User", email: "test@test.com", password: "Password.01", passwordConfirm: "Password.01" }).then(response => {
      expect(response.status).toEqual("ok");
    });

    const req = backend.expectOne("api/account/register");
    req.flush(new HttpResponse({
      status: 200
    }));
    backend.verify();
  });

  it('should attempt to get CSRF token and succeed', (done) => {
    //var spy = spyOn(service, "refreshAuth").and.returnValue(new LoginUserResponse("fail"));
    service.updateCsrfToken().then(response => {
      if (response != null) {
        expect(response.status).toEqual(200);
        done();
      }
    });
    
    const req = backend.expectOne("api/account/getCSRFToken");
    req.flush(new CsrfResponse("token", "token"));
    backend.verify();
  });

  it ("should verify that user is authenticated", () => {
    service.isAuth().then(response => {
      expect(response).toBe(true);
    });

    const req = backend.expectOne("api/account/isAuth");
    req.flush(1);
    backend.verify();
  });

});
