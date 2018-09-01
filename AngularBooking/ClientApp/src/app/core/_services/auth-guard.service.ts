import { Injectable } from '@angular/core';
import { Router, CanActivate, RouterStateSnapshot, ActivatedRouteSnapshot } from '@angular/router';
import { AuthService } from './auth.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthGuardService implements CanActivate {

  private loggedIn: boolean;

  constructor(private authService: AuthService, private router: Router) {
    this.authService.loggedIn.subscribe(data => {
      this.loggedIn = data;
    });
  }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | Promise<boolean> | Observable<boolean> {
    // get permitted role (if any) from route data
    const permittedRole = route.data.permittedRole;

    // get role information from auth service
    const role = this.authService.role.value;

    // check permittedRole is set, and if so check against role
    if (permittedRole) {
      if (permittedRole != role) {
        this.router.navigateByUrl("site");
        return false;
      }
    }

    // check user is logged in, and if not redirect to main site page
    if (!this.loggedIn) {
      this.router.navigateByUrl("site");
      return false;
    }

    return true;
  }



}
