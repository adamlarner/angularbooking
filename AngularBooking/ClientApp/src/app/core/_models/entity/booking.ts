import { BookingStatus } from "./booking-status";
import { Showing } from "./showing";
import { Customer } from "./customer";
import { BookingItem } from "./booking-item";

export class Booking {
  constructor(
    public id?: number,
    public bookedDate?: string,
    public status?: BookingStatus,
    public showingId?: number,
    public customerId?: number,
    public bookingItems?: BookingItem[]

  ) { }

  public totalPrice(): number {
    if (this.bookingItems) {
      var total = 0;
      for (let bookingItem of this.bookingItems) {
        total += bookingItem.agreedPrice;
      }
      return total;
    }
    return 0;
  }

}
