import { TestBed, inject, async } from '@angular/core/testing';

import { SiteService } from './site.service';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { ShowingListEntry } from '../_models/showing-list-entry';
import { Venue } from '../../core/_models/entity/venue';
import { Event } from '../../core/_models/entity/event';
import { Room } from '../../core/_models/entity/room';
import { AgeRatingType } from '../../core/_models/entity/age-rating-type';
import { Showing } from '../../core/_models/entity/showing';
import { FacilityFlags } from '../../core/_models/entity/facility-flags';
import { Booking } from '../../core/_models/entity/booking';
import { BookingStatus } from '../../core/_models/entity/booking-status';

describe('SiteService', () => {

  let backend: HttpTestingController;
  let service: SiteService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [
        HttpClientTestingModule,
        RouterTestingModule
      ],
      providers: [SiteService]
    });

    // get mocked instances of SUT
    backend = TestBed.get(HttpTestingController);
    service = TestBed.get(SiteService);

  });

  it('should be created', inject([SiteService], (service: SiteService) => {
    expect(service).toBeTruthy();
  }));

  it('should retrieve event from webapi', (done) => {
    service.getEvent(1).then(response => {
      expect(response).toEqual(new Event(1));
      done();
    });

    backend.expectOne("api/events/1").flush(
      new Event(1),
    );

    backend.verify();
  });

  it('should retrieve venue from webapi', (done) => {
    service.getVenue(1).then(response => {
      expect(response).toEqual(new Venue(1));
      done();
    });

    backend.expectOne("api/venues/1").flush(
      new Venue(1),
    );

    backend.verify();
  });

  it('should retrieve room from webapi', (done) => {
    service.getRoom(1).then(response => {
      expect(response).toEqual(new Room(1));
      done();
    });

    backend.expectOne("api/rooms/1").flush(
      new Room(1),
    );

    backend.verify();
  });


  it('should retrieve events from webapi', (done) => {
    service.getEvents("").then(response => {
      expect(response.length).toEqual(3);
      done();
    });

    backend.expectOne("api/events").flush([
      new Event(1),
      new Event(2),
      new Event(3)
    ]);

    backend.verify();
  });

  it('should retrieve venues from webapi', (done) => {
    service.getVenues().then(response => {
      expect(response.length).toEqual(3);
      done();
    });

    backend.expectOne("api/venues").flush([
      new Venue(1),
      new Venue(2),
      new Venue(3)
    ]);

    backend.verify();
  });

  it('should retrieve rooms from webapi', (done) => {
    service.getRooms().then(response => {
      expect(response.length).toEqual(3);
      done();
    });

    backend.expectOne("api/rooms").flush([
      new Room(1),
      new Room(2),
      new Room(3)
    ]);

    backend.verify();
  });

  it('should retrieve showing list entries filtered by event', (done) => {
    service.getShowingListing("event", 7, "2018-12-01", "2018-12-02").then(response => {
      expect(response.length).toEqual(1);
      expect(response[0].id).toEqual(2);
      expect(response[0].rooms.length).toEqual(1);
      expect(response[0].rooms[0].slots.length).toEqual(1);
      done();
    });

    // flush responses for getting venues and rooms
    var venueResponse: Venue[] = new Array<Venue>(new Venue(2, "test venue", "venue desc", "", "", "", "", "", "", "", "", "", "", "", "", FacilityFlags.None));
    var roomResponse: Room[] = new Array<Room>(new Room(1, 2, "test room", "room desc", 0, 0, "",
      new Array<Showing>(new Showing(2, 7, 1, "2018-12-01", "2018-12-02", 0))));

    const venueReq = backend.expectOne('api/venues?$select=Id,Name,Image,Address1,Address2,Address3,Address4,Address5,ContactPhone,Facilities');
    venueReq.flush(venueResponse);

    // simulate delay, so that next get request within service has been sent prior to next expectOne call *hack - being able to queue expected calls up front would work better*    
    setTimeout(() => {
      const roomReq = backend.expectOne(`api/rooms?$expand=Showings&$filter=Showings/any(s : date(s/StartTime) ge 2018-12-01 and date(s/StartTime) lt 2018-12-02 and s/EventId eq 7)&$select=Id,Name,Showings`);
      roomReq.flush(roomResponse);
    }, 3000);

    backend.verify();

  });

  it('should retrieve showing list entries filtered by venue', (done) => {
    service.getShowingListing("venue", 4, "2018-12-01", "2018-12-02").then(response => {
      expect(response.length).toEqual(1);
      expect(response[0].id).toEqual(3);
      expect(response[0].rooms.length).toEqual(1);
      expect(response[0].rooms[0].slots.length).toEqual(1);
      done();
    });

    // flush responses for getting events and rooms
    var eventResponse: Event[] = new Array<Event>(new Event(3, "test event", "event desc", "", 60, AgeRatingType.BBFC_12A));
    var roomResponse: Room[] = new Array<Room>(new Room(1, 4, "test room", "room desc", 0, 0, "",
      new Array<Showing>(new Showing(2, 3, 1, "2018-12-01", "2018-12-02", 0))));

    const eventReq = backend.expectOne('api/events?$select=Id,Name,Description,Image,Duration,AgeRating');
    eventReq.flush(eventResponse);

    // simulate delay, so that next get request within service has been sent prior to next expectOne call *hack - being able to queue expected calls up front would work better*    
    setTimeout(() => {
      const roomReq = backend.expectOne(`api/rooms?$expand=Showings&$filter=Showings/any(s : date(s/StartTime) ge 2018-12-01 and date(s/StartTime) lt 2018-12-02 and VenueId eq 4)&$select=Id,Name,Showings`);
      roomReq.flush(roomResponse);
    }, 3000);

    backend.verify();
  });

  it("should retrieve bookings for logged in user", (done) => {
    // user id is taken from backend
    service.getBookings().then(response => {
      expect(response.length).toEqual(2);
      done();
    });

    var bookingResponse: Booking[] = new Array<Booking>(
      new Booking(1, "2018-01-01", BookingStatus.PaymentComplete, 1, 1),
      new Booking(2, "2018-02-01", BookingStatus.PaymentComplete, 2, 1)
    );

    const bookingReq = backend.expectOne("api/bookings")
    bookingReq.flush(bookingResponse);

  })

});
