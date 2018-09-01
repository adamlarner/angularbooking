import { TestBed, inject } from '@angular/core/testing';

import { AdminService } from './admin.service';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { Event } from '../../core/_models/entity/event';
import { Venue } from '../../core/_models/entity/venue';
import { Room } from '../../core/_models/entity/room';
import { PricingStrategy } from '../../core/_models/entity/pricing-strategy';
import { PricingStrategyItem } from '../../core/_models/entity/pricing-strategy-item';
import { Booking } from '../../core/_models/entity/booking';
import { BookingItem } from '../../core/_models/entity/booking-item';
import { Showing } from '../../core/_models/entity/showing';
import { Customer } from '../../core/_models/entity/customer';

describe('AdminService', () => {
  let service: AdminService;
  let backend: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [
        HttpClientTestingModule,
        RouterTestingModule
      ],
      providers: [
        AdminService
      ]
    });

    // get mocked instances of SUT
    backend = TestBed.get(HttpTestingController);
    service = TestBed.get(AdminService);

  });

  it('should be created', inject([AdminService], (service: AdminService) => {
    expect(service).toBeTruthy();
  }));

  // events
  it('should create new event at Web API', (done) => {
    service.createEvent(new Event(1)).then(response => {
      expect(response).toBeTruthy();
      done();
    });

    backend.expectOne("api/events").flush(new Event(1), { status: 201, statusText: "Created" });
    backend.verify();

  });

  it('should update existing event at Web API', (done) => {
    service.updateEvent(new Event(1)).then(response => {
      expect(response).toEqual(true);
      done();
    });

    backend.expectOne("api/events/1").flush(1, { status: 204, statusText: "Updated" });
    backend.verify();
  });

  it('should delete event at Web API', (done) => {
    service.removeEvent(1).then(response => {
      expect(response).toEqual(true);     
      done();
    });

    backend.expectOne("api/events/1").flush(new Event(1), { status: 200, statusText: "Deleted" });
    backend.verify();
  });

  // venues
  it('should create new venue at Web API', (done) => {
    service.createVenue(new Venue(1)).then(response => {
      expect(response).toBeTruthy();
      done();
    });

    backend.expectOne("api/venues").flush(new Venue(1), { status: 201, statusText: "Created" });
    backend.verify();

  });

  it('should update existing venue at Web API', (done) => {
    service.updateVenue(new Venue(1)).then(response => {
      expect(response).toEqual(true);
      done();
    });

    backend.expectOne("api/venues/1").flush(1, { status: 204, statusText: "Updated" });
    backend.verify();
  });

  it('should delete venue at Web API', (done) => {
    service.removeVenue(1).then(response => {
      expect(response).toEqual(true);
      done();
    });

    backend.expectOne("api/venues/1").flush(new Venue(1), { status: 200, statusText: "Deleted" });
    backend.verify();
  });

  // rooms
  it('should create new room at Web API', (done) => {
    service.createRoom(new Room(1)).then(response => {
      expect(response).toBeTruthy();
      done();
    });

    backend.expectOne("api/rooms").flush(new Room(1), { status: 201, statusText: "Created" });
    backend.verify();

  });

  it('should update existing room at Web API', (done) => {
    service.updateRoom(new Room(1)).then(response => {
      expect(response).toEqual(true);
      done();
    });

    backend.expectOne("api/rooms/1").flush(1, { status: 204, statusText: "Updated" });
    backend.verify();
  });

  it('should delete room at Web API', (done) => {
    service.removeRoom(1).then(response => {
      expect(response).toEqual(true);
      done();
    });

    backend.expectOne("api/rooms/1").flush(new Room(1), { status: 200, statusText: "Deleted" });
    backend.verify();
  });

  // pricing strategies
  it('should create new pricing strategy at Web API', (done) => {
    service.createPricingStrategy(new PricingStrategy(1)).then(response => {
      expect(response).toBeTruthy();
      done();
    });

    backend.expectOne("api/pricingstrategies").flush(new PricingStrategy(1), { status: 201, statusText: "Created" });
    backend.verify();

  });

  it('should update existing pricing strategy at Web API', (done) => {
    service.updatePricingStrategy(new PricingStrategy(1)).then(response => {
      expect(response).toEqual(true);
      done();
    });

    backend.expectOne("api/pricingstrategies/1").flush(1, { status: 204, statusText: "Updated" });
    backend.verify();
  });

  it('should delete pricing strategy at Web API', (done) => {
    service.removePricingStrategy(1).then(response => {
      expect(response).toEqual(true);
      done();
    });

    backend.expectOne("api/pricingstrategies/1").flush(new PricingStrategy(1), { status: 200, statusText: "Deleted" });
    backend.verify();
  });

  // pricing strategy items
  it('should create new pricing strategy item at Web API', (done) => {
    service.createPricingStrategy(new PricingStrategyItem(1)).then(response => {
      expect(response).toBeTruthy();
      done();
    });

    backend.expectOne("api/pricingstrategies").flush(new PricingStrategyItem(1), { status: 201, statusText: "Created" });
    backend.verify();

  });

  it('should update existing pricing strategy at Web API', (done) => {
    service.updatePricingStrategy(new PricingStrategy(1)).then(response => {
      expect(response).toEqual(true);
      done();
    });

    backend.expectOne("api/pricingstrategies/1").flush(1, { status: 204, statusText: "Updated" });
    backend.verify();
  });

  it('should delete pricing strategy at Web API', (done) => {
    service.removePricingStrategy(1).then(response => {
      expect(response).toEqual(true);
      done();
    });

    backend.expectOne("api/pricingstrategies/1").flush(new PricingStrategy(1), { status: 200, statusText: "Deleted" });
    backend.verify();
  });

  // showings
  it('should create new showing at Web API', (done) => {
    service.createShowing(new Showing(1)).then(response => {
      expect(response).toBeTruthy();
      done();
    });

    backend.expectOne("api/showings").flush(new Showing(1), { status: 201, statusText: "Created" });
    backend.verify();

  });

  it('should update existing showing at Web API', (done) => {
    service.updateShowing(new Showing(1)).then(response => {
      expect(response).toEqual(true);
      done();
    });

    backend.expectOne("api/showings/1").flush(1, { status: 204, statusText: "Updated" });
    backend.verify();
  });

  it('should delete showing at Web API', (done) => {
    service.removeShowing(1).then(response => {
      expect(response).toEqual(true);
      done();
    });

    backend.expectOne("api/showings/1").flush(new Showing(1), { status: 200, statusText: "Deleted" });
    backend.verify();
  });

  // bookings
  it('should create new booking at Web API', (done) => {
    service.createBooking(new Booking(1)).then(response => {
      expect(response).toBeTruthy();
      done();
    });

    backend.expectOne("api/bookings").flush(new Booking(1), { status: 201, statusText: "Created" });
    backend.verify();

  });

  it('should update existing booking at Web API', (done) => {
    service.updateBooking(new Booking(1)).then(response => {
      expect(response).toEqual(true);
      done();
    });

    backend.expectOne("api/bookings/1").flush(1, { status: 204, statusText: "Updated" });
    backend.verify();
  });

  it('should delete booking at Web API', (done) => {
    service.removeBooking(1).then(response => {
      expect(response).toEqual(true);
      done();
    });

    backend.expectOne("api/bookings/1").flush(new Booking(1), { status: 200, statusText: "Deleted" });
    backend.verify();
  });

  // booking items
  it('should create new booking item at Web API', (done) => {
    service.createBookingItem(new BookingItem(1)).then(response => {
      expect(response).toBeTruthy();
      done();
    });

    backend.expectOne("api/bookingitems").flush(new BookingItem(1), { status: 201, statusText: "Created" });
    backend.verify();

  });

  it('should update existing booking item at Web API', (done) => {
    service.updateBookingItem(new BookingItem(1)).then(response => {
      expect(response).toEqual(true);
      done();
    });

    backend.expectOne("api/bookingitems/1").flush(1, { status: 204, statusText: "Updated" });
    backend.verify();
  });

  it('should delete booking item at Web API', (done) => {
    service.removeBookingItem(1).then(response => {
      expect(response).toEqual(true);
      done();
    });

    backend.expectOne("api/bookingitems/1").flush(new BookingItem(1), { status: 200, statusText: "Deleted" });
    backend.verify();
  });

  // customers
  it('should create new customer at Web API', (done) => {
    service.createCustomer(new Customer(1)).then(response => {
      expect(response).toBeTruthy();
      done();
    });

    backend.expectOne("api/customers").flush(new Customer(1), { status: 201, statusText: "Created" });
    backend.verify();

  });

  it('should update existing customer at Web API', (done) => {
    service.updateCustomer(new Customer(1)).then(response => {
      expect(response).toEqual(true);
      done();
    });

    backend.expectOne("api/customers/1").flush(1, { status: 204, statusText: "Updated" });
    backend.verify();
  });

  it('should delete customer at Web API', (done) => {
    service.removeCustomer(1).then(response => {
      expect(response).toEqual(true);
      done();
    });

    backend.expectOne("api/customers/1").flush(new Customer(1), { status: 200, statusText: "Deleted" });
    backend.verify();
  });

});
