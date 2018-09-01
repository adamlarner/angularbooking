import { TestBed, inject } from '@angular/core/testing';

import { RefreshInterceptorService } from './refresh-interceptor.service';
import { AuthService } from './auth.service';
import { HTTP_INTERCEPTORS, HttpClient, HttpHandler } from '@angular/common/http';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NgbModalStack } from '@ng-bootstrap/ng-bootstrap/modal/modal-stack';
import { RouterTestingModule } from '@angular/router/testing';
import { Ng4LoadingSpinnerModule } from 'ng4-loading-spinner';

describe('RefreshInterceptorService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [
        RouterTestingModule,
        Ng4LoadingSpinnerModule.forRoot()
      ],
      providers: [
        RefreshInterceptorService,
        HttpClientTestingModule,
        HttpClient,
        HttpHandler,
        AuthService,
        NgbModal, NgbModalStack, {
          provide: HTTP_INTERCEPTORS,
          useClass: RefreshInterceptorService,
          multi: true
        }
      ]
    });
  });

  it('should be created', inject([RefreshInterceptorService], (service: RefreshInterceptorService) => {
    expect(service).toBeTruthy();
  }));
});
