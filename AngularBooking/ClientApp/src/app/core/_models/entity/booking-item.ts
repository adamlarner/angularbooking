import { Booking } from "./booking";

export class BookingItem {

  constructor(
    public id?: number,
    public bookingId?: number,
    public location?: number,
    public agreedPrice?: number,
    public agreedPriceName?: string,
    
  ) { }
  
}
