import { Component, OnInit, Input } from '@angular/core';
import { ShowingListEntry } from '../_models/showing-list-entry';
import { AgeRatingType } from '../../core/_models/entity/age-rating-type';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-showing-list-entry',
  templateUrl: './showing-list-entry.component.html',
  styleUrls: ['./showing-list-entry.component.css']
})
export class ShowingListEntryComponent implements OnInit {

  constructor(private router: Router) { }

  @Input() model: ShowingListEntry = {  }

  ngOnInit() {
  }

  private ageRatingLookup: any[] = [
    "BBFC - UC",
    "BBFC - U",
    "BBFC - PG",
    "BBFC - 12A", 
    "BBFC - 12", 
    "BBFC - 15", 
    "BBFC - 18",
    "BBFC - R18", 
    "BBFC - TBC",
    "PEGI - Universal",
    "PEGI - PG", 
    "PEGI - 12A", 
    "PEGI - 12", 
    "PEGI - 15", 
    "PEGI - 18", 
    "PEGI - R18", 
  ];

  // flag check for facility
  hasFacility(flag) {
    return this.model.facilities & flag ? true : false;
  }

  // get age rating string
  lookupAgeRating(rating) {
    return this.ageRatingLookup[rating];
  }

  // click handler for going to booking site
  bookShowing(id) {
    this.router.navigateByUrl(`/booking/${id}`);
    return false;
  }

}
