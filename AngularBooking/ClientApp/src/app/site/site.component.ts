import { Component, OnInit } from '@angular/core';
import { AuthService } from '../core/_services/auth.service';
import { fadeAnimation } from '../core/_animations/fade-animation';
import { trigger, transition, style, animate, query } from "@angular/animations";

@Component({
  selector: 'app-site',
  templateUrl: './site.component.html',
  styleUrls: ['./site.component.css'],
  animations: [fadeAnimation],
  host: {
    '[@fadeAnimation]': ''
  }
})
export class SiteComponent implements OnInit {

  constructor(private authService: AuthService) { }

  ngOnInit() {
    // retrieve CSRF token from server
    this.authService.updateCsrfToken();

    // check for existing auth, and update
    this.authService.isAuth().then(authResponse => {
      if (authResponse) {
        this.authService.refreshAuth(true);
      }
    });
    
  }

}
