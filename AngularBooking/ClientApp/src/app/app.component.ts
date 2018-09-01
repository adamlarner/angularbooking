import { Component, OnInit } from '@angular/core';
import { fadeAnimation } from './core/_animations/fade-animation';
import { Title } from '@angular/platform-browser';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  animations: [fadeAnimation]
  
})
export class AppComponent implements OnInit {
  
  title = 'app';

  constructor(private titleService: Title) { }

  ngOnInit(): void {
    this.titleService.setTitle("Booking Application");
  }

}
