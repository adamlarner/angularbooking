import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { Venue } from '../../core/_models/entity/venue';
import { Event} from '../../core/_models/entity/event';
import { Room } from '../../core/_models/entity/room';
import { Showing } from '../../core/_models/entity/showing';

@Injectable({
  providedIn: 'root'
})
export class BookingService {

  constructor(private http: HttpClient) { }

  public async getAllocationData(id) {
    var allocationResponse: HttpResponse<number[]> = await this.http.get<number[]>(`api/showings/allocations/${id}`, {
      observe: "response"
    }).toPromise();

    // can't continue without allocations
    if (allocationResponse.status != 200)
      return null;

    return allocationResponse.body;
  }

  public async getShowingData(id) {
    // build return object
    var showingData = {
      allocations: [],
      showing: new Showing(),
      venue: new Venue(),
      isles: {}
    }
    
    // then get showing details (expand all relevant members)
    var showingResponse: HttpResponse<Showing[]> = await this.http.get<Showing[]>(`api/showings?$expand=room,event,pricingStrategy($expand=pricingStrategyItems)&$filter=id eq ${id}`, {
      observe: "response"
    }).toPromise();

    // no data, so return null
    if (showingResponse.body.length == 0)
      return null;

    showingData.showing = showingResponse.body[0];

    // get venue id from room, and get venue data
    var venueResponse: HttpResponse<Venue> = await this.http.get<Venue>(`api/venues/${showingData.showing.room.venueId}`, {
      observe: "response"
    }).toPromise();

    showingData.venue = venueResponse.body;

    // parse isles json string to object
    showingData.isles = JSON.parse(showingResponse.body[0].room.isles);

    return Promise.resolve(showingData);
  }

  async createBooking(bookingData): Promise<boolean> {
    try {
      var bookingResponse: HttpResponse<boolean> = await this.http.post<boolean>("api/bookings/create", bookingData, {
        observe: "response"
      }).toPromise();

      if (bookingResponse.status == 200)
        return Promise.resolve(true);

      return Promise.resolve(false);
    } catch (e) {
      return Promise.resolve(false);
    }
  }

}
