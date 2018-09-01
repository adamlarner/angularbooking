import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpSentEvent, HttpHeaderResponse, HttpProgressEvent, HttpResponse, HttpUserEvent, HttpEvent } from '@angular/common/http';
import { Observable, from, of } from 'rxjs';
import { AuthService } from './auth.service';
import { mergeMap, tap, catchError } from 'rxjs/operators';
import { LoginUserResponse } from '../_models/login-user-response';
import { Ng4LoadingSpinnerService } from 'ng4-loading-spinner';

@Injectable({
  providedIn: 'root'
})
export class RefreshInterceptorService implements HttpInterceptor {

  // keep track of number of in-progress requests
  private loadingCount: number = 0;

  constructor(private authService: AuthService, private spinnerService: Ng4LoadingSpinnerService) { }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpSentEvent | HttpHeaderResponse | HttpProgressEvent | HttpResponse<any> | HttpUserEvent<any>> {
    // increment loadingCount, since a request has been made
    this.loadingCount++;
    if (this.loadingCount == 1)
      this.spinnerService.show();

    var refreshObservable: Observable<LoginUserResponse> = from(this.authService.refreshAuth());

    // pipe additional operators for handling request after auth service is called
    return refreshObservable.pipe(
      mergeMap(result => {
        let request: Observable<HttpEvent<any>> = next.handle(req);

        // pipe catch and tap to request, so that the loadingCount can be incremented upon response/error
        return request.pipe(
          catchError((error: any) => {
            this.loadingCount--;
            if (this.loadingCount == 0)
              this.spinnerService.hide();

            return Observable.throw(error);
          }),
          tap(next => {
            if (next instanceof HttpResponse) {
              this.loadingCount--;
              if (this.loadingCount == 0)
                this.spinnerService.hide();
            }
          })
        );

      })
    );

  }
}
