import { Component, OnInit } from '@angular/core';
import { SiteService } from '../_services/site.service';
import { Venue } from '../../core/_models/entity/venue';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-venue-detail',
  templateUrl: './venue-detail.component.html',
  styleUrls: ['./venue-detail.component.css']
})
export class VenueDetailComponent implements OnInit {

  venue: Venue;

  constructor(private siteService: SiteService, private activatedRoute: ActivatedRoute) { }

  ngOnInit() {
    this.activatedRoute.params.subscribe(params => {
      this.siteService.getVenue(params['id']).then(v => {
        this.venue = v;
      });
    });
  }

  hasFacility(facilities, flag): boolean {
    return facilities & flag ? true : false;
  }

}
