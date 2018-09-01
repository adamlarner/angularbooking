import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpResponse } from '@angular/common/http';
import { ShowingListEntry } from '../_models/showing-list-entry';
import { Observable, BehaviorSubject } from 'rxjs';
import { ShowingListEntryRoom } from '../_models/showing-list-entry-room';
import { Venue } from '../../core/_models/entity/venue';
import { Event } from '../../core/_models/entity/event';
import { Room } from '../../core/_models/entity/room';
import { Customer } from '../../core/_models/entity/customer';
import { Booking } from '../../core/_models/entity/booking';
import { ShowingOptionGroup } from '../_models/showing-option-group';
import { Feature } from '../../core/_models/entity/feature';

@Injectable({
  providedIn: 'root'
})
export class SiteService {

  constructor(private http: HttpClient) { }

  public async getEvent(id: number): Promise<Event> {
    var response: HttpResponse<Event> = await this.http.get<Event>("api/events/" + id, {
      observe: "response"
    }).toPromise();

    if (response.status == 200) {
      return Promise.resolve(response.body);
    }

    return null;
  }

  public async getVenue(id: number): Promise<Venue> {
    var response: HttpResponse<Venue> = await this.http.get<Venue>("api/venues/" + id, {
      observe: "response"
    }).toPromise();

    if (response.status == 200) {
      return Promise.resolve(response.body);
    }

    return null;
  }

  public async getRoom(id: number): Promise<Room> {
    var response: HttpResponse<Room> = await this.http.get<Room>("api/rooms/" + id, {
      observe: "response"
    }).toPromise();

    if (response.status == 200) {
      return Promise.resolve(response.body);
    }

    return null;
  }

  public async getEvents(query?: string): Promise<Event[]> {
    var response: HttpResponse<Event[]> = await this.http.get<Event[]>("api/events" + (query ? query : ""), {
      observe: "response"
    }).toPromise();

    if (response.status == 200) {
      return Promise.resolve(response.body);
    }

    return [];
  }

  public async getVenues(query?: string): Promise<Event[]> {
    var response: HttpResponse<Venue[]> = await this.http.get<Venue[]>("api/venues" + (query ? query : ""), {
      observe: "response"
    }).toPromise();

    if (response.status == 200) {
      return Promise.resolve(response.body);
    }

    return [];
  }

  public async getRooms(query?: string): Promise<Room[]> {
    var response: HttpResponse<Room[]> = await this.http.get<Room[]>("api/rooms" + (query ? query : ""), {
      observe: "response"
    }).toPromise();

    if (response.status == 200) {
      return Promise.resolve(response.body);
    }

    return [];
  }

  public async getFeatures(): Promise<Feature[]> {
    var response: HttpResponse<Feature[]> = await this.http.get<Feature[]>("api/features", {
      observe: "response"
    }).toPromise();

    if (response.status == 200) {
      return Promise.resolve(response.body);
    }

    return [];
  }

  // showings
  public async getShowingListing(filterBy: string, filterId: number, startDate: string, endDate: string, fullDate?: boolean): Promise<ShowingListEntry[]> {
    if (filterBy == "event") {
      // retrieve all venues (set query to only select required details)
      var venueResponse: HttpResponse<Venue[]> = await this.http.get<Venue[]>("api/venues?$select=Id,Name,Image,Address1,Address2,Address3,Address4,Address5,ContactPhone,Facilities", {
        observe: "response"
      }).toPromise();

      // retrieve all rooms(including showings), between specified dates, and for specified event id
      var roomQuery = `api/rooms?$expand=Showings($filter=(date(StartTime) ge ${startDate}) and (date(StartTime) lt ${endDate})`
        + (filterId != null ? ` and (EventId eq ${filterId})` : "")
        + `)&$select=Id,VenueId,Name,Showings`;


      var roomResponse: HttpResponse<Room[]> = await this.http.get<Room[]>(roomQuery, {
          observe: "response"
      }).toPromise();

      
      // iterate each venue, then room, and allocate room/s to venue within showing list entry
      var showingListArray: ShowingListEntry[] = [];
      for (let v of venueResponse.body) {
        var showingListEntry: ShowingListEntry = {
          id: v.id,
          type: "venue",
          name: v.name,
          description: v.description,
          address1: v.address1,
          address2: v.address2,
          address3: v.address3,
          address4: v.address4,
          address5: v.address5,
          contact: v.contactPhone,
          image: v.image,
          facilities: v.facilities,
          rooms: []
        };

        for (let r of roomResponse.body) {
          // if room contains no showings, skip
          if (r.showings.length == 0)
            continue;

          // if room isn't within venue, skip
          if (r.venueId != v.id)
            continue;

          // map Room (and showings within) to ShowingListEntryRoom
          var room: ShowingListEntryRoom = {
            id: r.id,
            name: r.name,
            slots: []
          };

          for (let s of r.showings) {
            let showingDate: Date = new Date(s.startTime);
            
            if (!fullDate) {
              room.slots.push({
                id: s.id, time: showingDate.toLocaleTimeString([], { hour: "2-digit", minute: "2-digit", hour12: false, })
              });
            } else {
              room.slots.push({
                id: s.id, time: showingDate.toLocaleString()
              });
            }
            
          }

          // add room to entry
          showingListEntry.rooms.push(room);
        }

        // if entry contains at least one room, add to available showings array
        if (showingListEntry.rooms.length > 0)
          showingListArray.push(showingListEntry);
      }

      return Promise.resolve(showingListArray);

    }

    if (filterBy == "venue") {
      // retrieve all events that are shown within the venue
      var eventResponse: HttpResponse<Event[]> = await this.http.get<Event[]>("api/events?$select=Id,Name,Description,Image,Duration,AgeRating", {
        observe: "response"
      }).toPromise();
      
      // retrieve all rooms(including showings), between specified dates, and for specified event id
      var roomQuery = `api/rooms?$expand=Showings($expand=Room($select=VenueId);$filter=(date(StartTime) ge ${startDate}) and (date(StartTime) lt ${endDate})`
        + (filterId != null ? ` and (Room/VenueId eq ${filterId})` : "")
        + `)&$select=Id,Name,Showings`;
      
      var roomResponse: HttpResponse<Room[]> = await this.http.get<Room[]>(roomQuery, {
        observe: "response"
      }).toPromise();

      // iterate each venue, then room, and allocate room/s to venue within showing list entry
      var showingListArray: ShowingListEntry[] = [];
      for (let e of eventResponse.body) {
        var showingListEntry: ShowingListEntry = {
          id: e.id,
          type: "event",
          name: e.name,
          description: e.description,
          image: e.image,
          ageRating: e.ageRating,
          duration: e.duration.toString(),
          rooms: []
        };

        for (let r of roomResponse.body) {
          // if room contains no showings, skip
          if (r.showings.length == 0)
            continue;

          // if event isn't shown within venue , skip
          if (r.showings.findIndex((value, index) => {
            return value.eventId == e.id;
          }) == -1) {
            continue;
          }                
          
          // map Room (and showings within) to ShowingListEntryRoom
          var room: ShowingListEntryRoom = {
            id: r.id,
            name: r.name,
            slots: []
          };

          for (let s of r.showings) {
            if (e.id == s.eventId) {
              let showingDate: Date = new Date(s.startTime);
              if (!fullDate) {
                room.slots.push({
                  id: s.id, time: showingDate.toLocaleTimeString([], { hour: "2-digit", minute: "2-digit", hour12: false, })
                });
              } else {
                room.slots.push({
                  id: s.id, time: showingDate.toLocaleString()
                });
              }
            }

          }

          // add room to entry
          showingListEntry.rooms.push(room);
        }

        // if entry contains at least one room, add to available showings array
        if (showingListEntry.rooms.length > 0)
          showingListArray.push(showingListEntry);
      }

      return Promise.resolve(showingListArray);
    }

    return Promise.resolve([]);
  
  }

  // profile for current logged in user

  public async getProfile(): Promise<Customer> {
    try {
      // get current customer without querystring, will return current profile if authenticated
      var customerResponse: HttpResponse<Customer> = await this.http.get<Customer>("api/customer", {
        observe: "response"
      }).toPromise();

      if (customerResponse.status == 200) {
        return Promise.resolve(customerResponse.body);
      }
    } catch (e) {
      
    }
    
    // if issue getting profile, return null
    return null;
  }

  public async updateProfile(profile: Customer): Promise<boolean> {
    var customerResponse: any = await this.http.put("api/customer", profile, {
      observe: "response"
    }).toPromise();

    return Promise.resolve(customerResponse.status == 204);
  }
  
  // bookings for current logged in user

  public async getBookings(includeItems?: boolean): Promise<Booking[]> {
    try {
      var bookingResponse: HttpResponse<Booking[]> = await this.http.get<Booking[]>(`api/bookings${includeItems ? "?$expand=bookingItems" : ""}`, {
        observe: "response"
      }).toPromise();

      if (bookingResponse.status == 200) {
        // map json response to actual Booking object
        var bookings = bookingResponse.body.map(f => {
          return new Booking(f.id, f.bookedDate, f.status, f.showingId, f.customerId, f.bookingItems);
        });
        return Promise.resolve(bookings);
      }
    } catch (e) {
      
    }
    
    // issue getting bookings, so return null
    return null;
  }

  public async cancelBooking(id: number): Promise<boolean> {
    try {
      var cancelResponse: HttpResponse<boolean> = await this.http.post<boolean>("api/bookings/cancel", id, {
        observe: "response"
      }).toPromise();

      if (cancelResponse.status == 200) {
        return Promise.resolve(cancelResponse.body);
      }

    } catch (e) {

    }
  }

  public async getShowingAllocations(showingId: number): Promise<number[]> {
    try {
      var allocationResponse: HttpResponse<number[]> = await this.http.get<number[]>(`api/showings/allocations/${showingId}`, {
        observe: "response"
      }).toPromise();

      if (allocationResponse.status == 200) {
        return Promise.resolve(allocationResponse.body);
      }

    } catch (e) {

    }
  }

  public async getShowingOptions(): Promise<ShowingOptionGroup[]> {
    // retrieve all events, select only id and name (needed for lookup)
    var eventResponse: HttpResponse<Event[]> = await this.http.get<Event[]>("api/events?$select=Id,Name", {
      observe: "response"
    }).toPromise();

    // same as above, except for venues
    var venueResponse: HttpResponse<Venue[]> = await this.http.get<Venue[]>("api/venues?$select=Id,Name", {
      observe: "response"
    }).toPromise();

    // retrieve all rooms
    var roomQuery = `api/rooms?$expand=Showings&$select=Id,VenueId,Name,Showings,Rows,Columns`;
    var roomResponse: HttpResponse<Room[]> = await this.http.get<Room[]>(roomQuery, {
      observe: "response"
    }).toPromise();

    // iterate each event, then room, and create showing option (todo: switch iteration order and sort)
    var showingOptionGroupArray: ShowingOptionGroup[] = [];
    for (let e of eventResponse.body) {

      for (let r of roomResponse.body) {
        // if room contains no showings, skip
        if (r.showings.length == 0)
          continue;

        // get venue name from venue response
        var venue = venueResponse.body.find(f => f.id == r.venueId);

        // problem finding venue, so ignore
        if (!venue)
          continue;

        var showingOptionGroup: ShowingOptionGroup = {
          event: e.name,
          venue: venue.name,
          showingOptions: []
        };

        for (let s of r.showings) {
          if (s.eventId != e.id)
            continue;

          let showingDate: Date = new Date(s.startTime);
          showingOptionGroup.showingOptions.push({
            id: s.id,
            room: r.name,
            time: showingDate.toLocaleString(),
            rows: r.rows,
            columns: r.columns
          });
        }

        showingOptionGroupArray.push(showingOptionGroup);
      }

    }

    return Promise.resolve(showingOptionGroupArray);

  }


}
