import { BookingRoutingModule } from './booking-routing.module';

describe('BookingRoutingModule', () => {
  let bookingRoutingModule: BookingRoutingModule;

  beforeEach(() => {
    bookingRoutingModule = new BookingRoutingModule();
  });

  it('should create an instance', () => {
    expect(bookingRoutingModule).toBeTruthy();
  });
});
