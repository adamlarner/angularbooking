import { Component, OnInit } from '@angular/core';
import { fadeAnimation } from '../../core/_animations/fade-animation';


@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css'],
  animations: [fadeAnimation]
})
export class NavbarComponent implements OnInit {

  isNavbarCollapsed: boolean;

  constructor() { }

  ngOnInit() {
  }

}
