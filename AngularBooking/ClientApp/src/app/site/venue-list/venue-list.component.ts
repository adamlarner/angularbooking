import { Component, OnInit } from '@angular/core';
import { SiteService } from '../_services/site.service';
import { Venue } from '../../core/_models/entity/venue';

@Component({
  selector: 'app-venue-list',
  templateUrl: './venue-list.component.html',
  styleUrls: ['./venue-list.component.css']
})
export class VenueListComponent implements OnInit {

  venues: Venue[];

  constructor(private siteService: SiteService) { }

  ngOnInit() {
    this.siteService.getVenues().then(v => {
      this.venues = v;
    });
  }

  hasFacility(facilities, flag): boolean {
    return facilities & flag ? true : false;
  }

}
