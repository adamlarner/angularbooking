import { Component, OnInit, Input } from '@angular/core';
import { ShowingListEntry } from '../_models/showing-list-entry';
import { SiteService } from '../_services/site.service';
import { Observable, BehaviorSubject } from 'rxjs';
import { NgbDatepicker, NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';
import { NgbDateStructAdapter } from '@ng-bootstrap/ng-bootstrap/datepicker/adapters/ngb-date-adapter';
import { Venue } from '../../core/_models/entity/venue';
import { Event } from '../../core/_models/entity/event';

@Component({
  selector: 'app-showing-list',
  templateUrl: './showing-list.component.html',
  styleUrls: ['./showing-list.component.css'],
  host: { "class": "component-routing-modifier" }
})
export class ShowingListComponent implements OnInit {

  constructor(private siteService: SiteService) { }

  filterBy: string = "event";
  filterId: number = 0;
  filterDate: NgbDateStruct;

  venueList: Venue[];
  eventList: Event[];

  entries: ShowingListEntry[];

  ngOnInit() {
    // set up filter state
    var todayDate: Date = new Date(Date.now());
    this.filterDate = { day: todayDate.getDate(), month: todayDate.getMonth() + 1, year: todayDate.getFullYear() };

    // get event/venue lists
    this.siteService.getEvents("?$select=Id,Name").then((events => {
      this.siteService.getVenues("?$select=Id,Name").then(venues => {
        this.eventList = events;
        this.venueList = venues;
        // populate initial listing
        this.populateListing();
      });
    }));

  }

  onFilterChange(change): void {
    // update listing
    this.populateListing();
  }
  
  setFilterDateToday(datepicker: NgbDatepicker): void {
    var todayDate: Date = new Date(Date.now());
    this.filterDate = { day: todayDate.getDate(), month: todayDate.getMonth() + 1, year: todayDate.getFullYear() };
    this.populateListing();
  }

  setFilterDateTomorrow(datepicker: NgbDatepicker): void {
    var tomorrowDate: Date = new Date(Date.now());
    tomorrowDate.setDate(tomorrowDate.getDate() + 1);
    this.filterDate = { day: tomorrowDate.getDate(), month: tomorrowDate.getMonth() + 1, year: tomorrowDate.getFullYear() };
    this.populateListing();
  }

  private populateListing(): void {
    var startDate = new Date(this.filterDate.year, this.filterDate.month - 1, this.filterDate.day).toISOString();
    var endDate = new Date(this.filterDate.year, this.filterDate.month - 1, this.filterDate.day + 1).toISOString();

    this.siteService.getShowingListing(this.filterBy, this.filterId, startDate, endDate).then((showingList) => {
      this.entries = showingList;
    }).catch(e => {
    });
    
  }

}
