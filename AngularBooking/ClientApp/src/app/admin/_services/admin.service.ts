import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { SiteService } from '../../site/_services/site.service';
import { Venue } from '../../core/_models/entity/venue';
import { Event } from '../../core/_models/entity/event';
import { Room } from '../../core/_models/entity/room';
import { PricingStrategy } from '../../core/_models/entity/pricing-strategy';
import { PricingStrategyItem } from '../../core/_models/entity/pricing-strategy-item';
import { Booking } from '../../core/_models/entity/booking';
import { BookingItem } from '../../core/_models/entity/booking-item';
import { Showing } from '../../core/_models/entity/showing';
import { Customer } from '../../core/_models/entity/customer';
import { ShowingOptionGroup } from '../../site/_models/showing-option-group';
import { Feature } from '../../core/_models/entity/feature';

@Injectable({
  providedIn: 'root'
})
export class AdminService {

  // some 'get' functions within SiteService, so consuming that rather than re-implementing
  constructor(private http: HttpClient, private siteService: SiteService) { }

  // event
  async createEvent(event: Event): Promise<Event> {
    var response: HttpResponse<Event> = await this.http.post("api/events", event, {
      observe: "response"
    }).toPromise();

    return Promise.resolve(response.body);
  }

  async updateEvent(event: Event): Promise<boolean> {
    var response: HttpResponse<any> = await this.http.put("api/events/" + event.id, event, {
      observe: "response"
    }).toPromise();

    return Promise.resolve(response.status == 204);
  }

  async removeEvent(id: number): Promise<boolean> {
    var response: HttpResponse<any> = await this.http.delete("api/events/" + id, {
      observe: "response"
    }).toPromise();

    return Promise.resolve(response.status == 200);
  }

  async getEvent(id: number): Promise<Event> {
    return this.siteService.getEvent(id);
  }

  async getEvents(query: string): Promise<Event[]> {
    return this.siteService.getEvents(query);
  }

  // venue
  async createVenue(venue: Venue): Promise<Venue> {
    var response: HttpResponse<Venue> = await this.http.post("api/venues", venue, {
      observe: "response"
    }).toPromise();

    return Promise.resolve(response.body);
  }

  async updateVenue(venue: Venue): Promise<boolean> {
    var response: HttpResponse<any> = await this.http.put("api/venues/" + venue.id, venue, {
      observe: "response"
    }).toPromise();

    return Promise.resolve(response.status == 204);
  }

  async removeVenue(id: number): Promise<boolean> {
    var response: HttpResponse<any> = await this.http.delete("api/venues/" + id, {
      observe: "response"
    }).toPromise();

    return Promise.resolve(response.status == 200);
  }

  async getVenue(id: number): Promise<Venue> {
    return this.siteService.getVenue(id);
  }

  async getVenues(query: string): Promise<Venue[]> {
    return this.siteService.getVenues(query);
  }

  // room
  async createRoom(room: Room): Promise<Room> {
    var response: HttpResponse<Room> = await this.http.post("api/rooms", room, {
      observe: "response"
    }).toPromise();

    return Promise.resolve(response.body);
  }

  async updateRoom(room: Room): Promise<boolean> {
    var response: HttpResponse<any> = await this.http.put("api/rooms/" + room.id, room, {
      observe: "response"
    }).toPromise();

    return Promise.resolve(response.status == 204);
  }

  async removeRoom(id: number): Promise<boolean> {
    var response: HttpResponse<any> = await this.http.delete("api/rooms/" + id, {
      observe: "response"
    }).toPromise();

    return Promise.resolve(response.status == 200);
  }

  async getRoom(id: number): Promise<Room> {
    return this.siteService.getRoom(id);
  }

  async getRooms(query: string): Promise<Room[]> {
    return this.siteService.getRooms(query);
  }

  // pricing strategy
  async createPricingStrategy(pricingStrategy: PricingStrategy): Promise<PricingStrategy> {
    var response: HttpResponse<PricingStrategy> = await this.http.post("api/pricingstrategies", pricingStrategy, {
      observe: "response"
    }).toPromise();

    return Promise.resolve(response.body);
  }

  async updatePricingStrategy(pricingStrategy: PricingStrategy): Promise<boolean> {
    var response: HttpResponse<any> = await this.http.put("api/pricingstrategies/" + pricingStrategy.id, pricingStrategy, {
      observe: "response"
    }).toPromise();

    return Promise.resolve(response.status == 204);
  }

  async removePricingStrategy(id: number): Promise<boolean> {
    var response: HttpResponse<any> = await this.http.delete("api/pricingstrategies/" + id, {
      observe: "response"
    }).toPromise();

    return Promise.resolve(response.status == 200);
  }

  async getPricingStrategy(id: number): Promise<PricingStrategy> {
    var response: HttpResponse<PricingStrategy> = await this.http.get<PricingStrategy>("api/pricingstrategies/" + id, {
      observe: "response"
    }).toPromise();

    if (response.status == 200) {
      return Promise.resolve(response.body);
    }

    return null;
  }

  async getPricingStrategies(query: string): Promise<PricingStrategy[]> {
    var response: HttpResponse<PricingStrategy[]> = await this.http.get<PricingStrategy[]>("api/pricingstrategies" + (query ? query : ""), {
      observe: "response"
    }).toPromise();

    if (response.status == 200) {
      return Promise.resolve(response.body);
    }

    return [];
  }

  // pricing strategy items
  async createPricingStrategyItem(pricingStrategyItem: PricingStrategyItem): Promise<PricingStrategyItem> {
    var response: HttpResponse<PricingStrategyItem> = await this.http.post("api/pricingstrategyitems", pricingStrategyItem, {
      observe: "response"
    }).toPromise();

    return Promise.resolve(response.body);
  }

  async updatePricingStrategyItem(pricingStrategyItem: PricingStrategyItem): Promise<boolean> {
    var response: HttpResponse<any> = await this.http.put("api/pricingstrategyitems/" + pricingStrategyItem.id, pricingStrategyItem, {
      observe: "response"
    }).toPromise();

    return Promise.resolve(response.status == 204);
  }

  async removePricingStrategyItem(id: number): Promise<boolean> {
    var response: HttpResponse<any> = await this.http.delete("api/pricingstrategyitems/" + id, {
      observe: "response"
    }).toPromise();

    return Promise.resolve(response.status == 200);
  }

  async getPricingStrategyItem(id: number): Promise<PricingStrategyItem> {
    var response: HttpResponse<PricingStrategyItem> = await this.http.get<PricingStrategyItem>("api/pricingstrategyitems/" + id, {
      observe: "response"
    }).toPromise();

    if (response.status == 200) {
      return Promise.resolve(response.body);
    }

    return null;
  }

  async getPricingStrategyItems(query: string): Promise<PricingStrategyItem[]> {
    var response: HttpResponse<PricingStrategyItem[]> = await this.http.get<PricingStrategyItem[]>("api/pricingstrategyitems" + (query ? query : ""), {
      observe: "response"
    }).toPromise();

    if (response.status == 200) {
      return Promise.resolve(response.body);
    }

    return [];
  }

  // showings
  async createShowing(showing: Showing): Promise<Showing> {
    var response: HttpResponse<Showing> = await this.http.post("api/showings", showing, {
      observe: "response"
    }).toPromise();

    return Promise.resolve(response.body);
  }

  async updateShowing(showing: Showing): Promise<boolean> {
    var response: HttpResponse<any> = await this.http.put("api/showings/" + showing.id, showing, {
      observe: "response"
    }).toPromise();

    return Promise.resolve(response.status == 204);
  }

  async removeShowing(id: number): Promise<boolean> {
    var response: HttpResponse<any> = await this.http.delete("api/showings/" + id, {
      observe: "response"
    }).toPromise();

    return Promise.resolve(response.status == 200);
  }

  async getShowing(id: number): Promise<Showing> {
    var response: HttpResponse<Showing> = await this.http.get<Showing>("api/showings/" + id, {
      observe: "response"
    }).toPromise();

    if (response.status == 200) {
      return Promise.resolve(response.body);
    }

    return null;
  }

  async getShowings(query: string): Promise<Showing[]> {
    var response: HttpResponse<Showing[]> = await this.http.get<Showing[]>("api/showings" + (query ? query : ""), {
      observe: "response"
    }).toPromise();

    if (response.status == 200) {
      return Promise.resolve(response.body);
    }

    return [];
  }

  // use site service method
  public async getShowingAllocations(showingId: number): Promise<number[]> {
    return await this.siteService.getShowingAllocations(showingId);
  }

  // bookings
  async createBooking(booking: Booking): Promise<Booking> {
    var response: HttpResponse<Booking> = await this.http.post<Booking>("api/bookings", booking, {
      observe: "response"
    }).toPromise();

    return Promise.resolve(response.body);
  }

  async updateBooking(booking: Booking): Promise<boolean> {
    var response: HttpResponse<any> = await this.http.put("api/bookings/" + booking.id, booking, {
      observe: "response"
    }).toPromise();

    return Promise.resolve(response.status == 204);
  }

  async removeBooking(id: number): Promise<boolean> {
    var response: HttpResponse<any> = await this.http.delete("api/bookings/" + id, {
      observe: "response"
    }).toPromise();

    return Promise.resolve(response.status == 200);
  }

  async getBooking(id: number): Promise<Booking> {
    var response: HttpResponse<Booking> = await this.http.get<Booking>("api/bookings/" + id, {
      observe: "response"
    }).toPromise();

    if (response.status == 200) {
      return Promise.resolve(response.body);
    }

    return null;
  }

  async getBookings(query: string): Promise<Booking[]> {
    var response: HttpResponse<Booking[]> = await this.http.get<Booking[]>("api/bookings" + (query ? query : ""), {
      observe: "response"
    }).toPromise();

    if (response.status == 200) {
      return Promise.resolve(response.body);
    }

    return [];
  }

  // booking items
  async createBookingItem(bookingItem: BookingItem): Promise<BookingItem> {
    var response: HttpResponse<BookingItem> = await this.http.post("api/bookingitems", bookingItem, {
      observe: "response"
    }).toPromise();

    return Promise.resolve(response.body);
  }

  async updateBookingItem(bookingItem: BookingItem): Promise<boolean> {
    var response: HttpResponse<any> = await this.http.put("api/bookingitems/" + bookingItem.id, bookingItem, {
      observe: "response"
    }).toPromise();
    
    return Promise.resolve(response.status == 204);
  }

  async removeBookingItem(id: number): Promise<boolean> {
    var response: HttpResponse<any> = await this.http.delete("api/bookingitems/" + id, {
      observe: "response"
    }).toPromise();

    return Promise.resolve(response.status == 200);
  }

  async getBookingItem(id: number): Promise<BookingItem> {
    var response: HttpResponse<BookingItem> = await this.http.get<BookingItem>("api/bookingitems/" + id, {
      observe: "response"
    }).toPromise();

    if (response.status == 200) {
      return Promise.resolve(response.body);
    }

    return null;
  }

  async getBookingItems(query: string): Promise<BookingItem[]> {
    var response: HttpResponse<BookingItem[]> = await this.http.get<BookingItem[]>("api/bookingitems" + (query ? query : ""), {
      observe: "response"
    }).toPromise();

    if (response.status == 200) {
      return Promise.resolve(response.body);
    }

    return [];
  }

  // customers
  async createCustomer(customer: Customer): Promise<Customer> {
    var response: HttpResponse<Customer> = await this.http.post("api/customers", customer, {
      observe: "response"
    }).toPromise();

    return Promise.resolve(response.body);
  }

  async updateCustomer(customer: Customer): Promise<boolean> {
    var response: HttpResponse<any> = await this.http.put("api/customers/" + customer.id, customer, {
      observe: "response"
    }).toPromise();

    return Promise.resolve(response.status == 204);
  }

  async removeCustomer(id: number): Promise<boolean> {
    var response: HttpResponse<any> = await this.http.delete("api/customers/" + id, {
      observe: "response"
    }).toPromise();

    return Promise.resolve(response.status == 200);
  }

  async getCustomer(id: number): Promise<Customer> {
    var response: HttpResponse<Customer> = await this.http.get<Customer>("api/customers/" + id, {
      observe: "response"
    }).toPromise();

    if (response.status == 200) {
      return Promise.resolve(response.body);
    }

    return null;
  }

  async getCustomers(query: string): Promise<Customer[]> {
    var response: HttpResponse<Customer[]> = await this.http.get<Customer[]>("api/customers" + (query ? query : ""), {
      observe: "response"
    }).toPromise();

    if (response.status == 200) {
      return Promise.resolve(response.body);
    }

    return [];
  }

  // features
  async createFeature(feature: Feature): Promise<Feature> {
    var response: HttpResponse<Feature> = await this.http.post("api/features", feature, {
      observe: "response"
    }).toPromise();

    return Promise.resolve(response.body);
  }

  async updateFeature(feature: Feature): Promise<boolean> {
    var response: HttpResponse<any> = await this.http.put("api/features/" + feature.id, feature, {
      observe: "response"
    }).toPromise();

    return Promise.resolve(response.status == 204);
  }

  async removeFeature(id: number): Promise<boolean> {
    var response: HttpResponse<any> = await this.http.delete("api/features/" + id, {
      observe: "response"
    }).toPromise();

    return Promise.resolve(response.status == 200);
  }

  async getFeature(id: number): Promise<Feature> {
    var response: HttpResponse<Feature> = await this.http.get<Feature>("api/features/" + id, {
      observe: "response"
    }).toPromise();

    if (response.status == 200) {
      return Promise.resolve(response.body);
    }

    return null;
  }

  async getFeatures(query: string): Promise<Feature[]> {
    var response: HttpResponse<Feature[]> = await this.http.get<Feature[]>("api/features" + (query ? query : ""), {
      observe: "response"
    }).toPromise();

    if (response.status == 200) {
      return Promise.resolve(response.body);
    }

    return [];
  }

  // showing (composite detail option, containing event, room and venue names)

  public async getShowingOptions(): Promise<ShowingOptionGroup[]> {
    return this.siteService.getShowingOptions();
  }

}
