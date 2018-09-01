import { Component, OnInit } from '@angular/core';
import { FormGroup, Validators, FormControl, AbstractControl } from '@angular/forms';
import { AdminService } from '../_services/admin.service';
import { Booking } from '../../core/_models/entity/booking';
import { BookingItem } from '../../core/_models/entity/booking-item';
import { BookingStatus } from '../../core/_models/entity/booking-status';
import { Customer } from '../../core/_models/entity/customer';
import { Venue } from '../../core/_models/entity/venue';
import { SiteService } from '../../site/_services/site.service';
import { ShowingOptionGroup } from '../../site/_models/showing-option-group';
import { MessageService } from 'primeng/components/common/messageservice';

@Component({
  selector: 'app-bookings',
  templateUrl: './bookings.component.html',
  styleUrls: ['./bookings.component.css']
})
export class BookingsComponent implements OnInit {

  // booking state members
  newBooking: boolean;
  bookingForm: FormGroup;
  displayBookingDialog: boolean;
  selectedBooking: Booking;
  bookings: Booking[] = [];
  bookingCols: any[];
  filterId: number;

  // booking item state members
  newBookingItem: boolean;
  bookingItemForm: FormGroup;
  displayBookingItemDialog: boolean;
  selectedBookingItem: BookingItem;
  bookingItems: BookingItem[] = [];
  selectedBookingId: number;

  customers: Customer[];
  showings: ShowingOptionGroup[];
  // available/allocated locations
  availability: { location: string, value: number }[];
  allocations: { location: string, value: number }[];

  constructor(private adminService: AdminService, private messageService: MessageService) { }

  ngOnInit() {
    // configure add/edit form (Booking)
    this.bookingForm = new FormGroup({
      'id': new FormControl(0, [
        Validators.required
      ]),
      'bookedDate': new FormControl("1970-1-1", []),
      'status': new FormControl(BookingStatus.PaymentPending, [
        Validators.required
      ]),
      'customerId': new FormControl("", [
        Validators.required
      ]),
      'showingId': new FormControl("", [
        Validators.required
      ])
    });

    // configure add/edit form (BookingItem)
    this.bookingItemForm = new FormGroup({
      'id': new FormControl(0, [
        Validators.required
      ]),
      'location': new FormControl(null, [
        Validators.required
      ]),
      'agreedPrice': new FormControl(0, [
        Validators.required
      ]),
      'agreedPriceName': new FormControl("", [
        Validators.required
      ])
    });

    // get booking data from service
    this.adminService.getBookings("").then((e => {
      this.bookings = e;
    }));

    // get booking items data from service
    this.adminService.getBookingItems("").then((e => {
      this.bookingItems = e;
    }));

    // get customer listings (id + name)
    this.adminService.getCustomers("?$select=Id,FirstName,LastName").then(e => {
      this.customers = e;
    });

    // get showings
    this.adminService.getShowingOptions().then(e => {
      this.showings = e;
    });

    // set up column defs for table
    this.bookingCols = [
      { field: 'name', header: "Name" }
    ];

  }

  /*
   * Booking CRUD methods
   **/

  showDialogToAddBooking() {
    this.selectedBooking = null;
    this.newBooking = true;
    this.bookingForm.reset({ 'id': 0 });
    this.displayBookingDialog = true;
  }

  async saveBooking(): Promise<void> {

    // check form data is valid
    if (!this.bookingForm.valid)
      return;

    let bookings = [...this.bookings];

    // create new booking from form data
    let booking = new Booking(
      this.bookingForm.controls.id.value,
      "1970-1-1",
      this.bookingForm.controls.status.value,
      this.bookingForm.controls.showingId.value,
      this.bookingForm.controls.customerId.value
    );

    if (this.newBooking) {
      // attempt post to service, then push to array on success
      var postSuccess = await this.adminService.createBooking(booking);
      if (postSuccess) {
        bookings.push(postSuccess);
        this.messageService.add({ severity: "success", summary: "Booking Created!", life: 2500 });
      } else {
        // display error message (todo: pretty)
        alert("error!");
      }
    }
    else {
      // attempt put to service
      var putSuccess = await this.adminService.updateBooking(booking);
      if (putSuccess) {
        bookings[this.bookings.indexOf(this.selectedBooking)] = booking;
        this.messageService.add({ severity: "success", summary: "Booking Modified!", life: 2500 });
      } else {
        // display error message (todo: pretty)
        alert("error!");
      }
    }

    this.bookings = bookings;
    this.displayBookingDialog = false;
  }

  async deleteBooking() {
    if (!this.selectedBooking)
      return;

    // attempt delete at service
    var deleteSuccess = await this.adminService.removeBooking(this.selectedBooking.id);

    if (deleteSuccess) {
      let index = this.bookings.indexOf(this.selectedBooking);
      this.bookings = this.bookings.filter((val, i) => i != index);
      this.displayBookingDialog = false;
      this.messageService.add({ severity: "success", summary: "Booking Removed!", life: 2500 });
    }
  }

  onBookingRowEdit(booking) {
    this.selectedBooking = booking.data;
    this.newBooking = false;
    this.bookingForm.controls.id.setValue(booking.data.id);
    this.bookingForm.controls.bookedDate.setValue(booking.data.bookedDate);
    this.bookingForm.controls.status.setValue(booking.data.status);
    this.bookingForm.controls.showingId.setValue(booking.data.showingId);
    this.bookingForm.controls.customerId.setValue(booking.data.customerId);
    this.displayBookingDialog = true;

    return false;
  }

  /*
   * Booking Item CRUD methods
   **/

  async showDialogToAddBookingItem(bookingId): Promise<void> {
    this.selectedBookingItem = null;
    this.newBookingItem = true;
    this.bookingItemForm.reset({ 'id': 0 });
    this.selectedBookingId = bookingId;
    this.displayBookingItemDialog = true;

    this.updateAvailability(bookingId);

  }

  async saveBookingItem(): Promise<void> {

    // check form data is valid
    if (!this.bookingItemForm.valid)
      return;

    let bookingItems = [...this.bookingItems];

    // create new booking item from form data
    let bookingItem = new BookingItem(
      this.bookingItemForm.controls.id.value,
      this.selectedBookingId,
      this.bookingItemForm.controls.location.value,
      this.bookingItemForm.controls.agreedPrice.value,
      this.bookingItemForm.controls.agreedPriceName.value
    );

    if (this.newBookingItem) {
      // attempt post to service, then push to array on success
      var postSuccess = await this.adminService.createBookingItem(bookingItem);
      if (postSuccess) {
        bookingItems.push(postSuccess);
        this.messageService.add({ severity: "success", summary: "Booking Item Created!", life: 2500 });
      } else {
        // display error message (todo: pretty)
        alert("error!");
      }
    }
    else {
      // attempt put to service
      var putSuccess = await this.adminService.updateBookingItem(bookingItem);
      if (putSuccess) {
        bookingItems[this.bookingItems.findIndex(f => f.id == this.selectedBookingItem.id)] = bookingItem;
        this.messageService.add({ severity: "success", summary: "Booking Item Modified!", life: 2500 });
      } else {
        // display error message (todo: pretty)
        alert("error!");
      }
    }

    this.bookingItems = bookingItems;
    this.displayBookingItemDialog = false;
  }

  async deleteBookingItem() {
    if (!this.selectedBookingItem)
      return;

    // attempt delete at service
    var deleteSuccess = await this.adminService.removeBookingItem(this.selectedBookingItem.id);

    if (deleteSuccess) {
      let index = this.bookingItems.findIndex(f => f.id == this.selectedBookingItem.id);
      this.bookingItems = this.bookingItems.filter((val, i) => i != index);
      this.displayBookingItemDialog = false;
      this.messageService.add({ severity: "success", summary: "Booking Item Removed!", life: 2500 });
    }
  }

  async onBookingItemRowEdit(bookingItem, bookingId): Promise<boolean> {
    await this.updateAvailability(bookingId);

    // since the record is being editted, include original selection in availability list
    var current = this.allocations.find(f => f.value == bookingItem.data.location);
    if (current)
      this.availability.push({ location: current.location, value: current.value });

    this.selectedBookingItem = bookingItem.data;
    this.selectedBookingId = bookingId;
    this.newBookingItem = false;
    this.bookingItemForm.controls.id.setValue(bookingItem.data.id);
    this.bookingItemForm.controls.location.setValue(bookingItem.data.location);
    this.bookingItemForm.controls.agreedPrice.setValue(bookingItem.data.agreedPrice);
    this.bookingItemForm.controls.agreedPriceName.setValue(bookingItem.data.agreedPriceName);
    this.displayBookingItemDialog = true;

    return false;
  }

  getBookingItemsByBookingId(id) {
    // get booking, then showing option, then room rows/cols for calculating location name
    let booking = this.bookings.find(f => f.id == id);
    let showingOption = this.showings.map(f => {
      return f.showingOptions.find(g => g.id == booking.showingId);
    }).filter(f => f != null);

    if (showingOption.length == 0)
      return [];

    return this.bookingItems.filter(f => f.bookingId == id).map(f => {

      var locationRow = Math.floor(f.location / showingOption[0].columns);
      var locationColumn = f.location % showingOption[0].columns;
      var locationName = this.calculateLocationName(locationRow, locationColumn);

      return {
        id: f.id,
        bookingId: f.bookingId,
        location: f.location,
        locationName: locationName,
        agreedPrice: f.agreedPrice,
        agreedPriceName: f.agreedPriceName
      };
    });
  }

  // return flat object detailing showing
  getShowingInfoById(id) {
    if (!this.showings)
      return {
        event: "unknown",
        venue: "unknown",
        room: "unknown",
        time: "unknown",
      };

    for (var showingOptionGroup of this.showings) {
      for (var showingOption of showingOptionGroup.showingOptions) {
        if (showingOption.id == id)
          return {
            event: showingOptionGroup.event,
            venue: showingOptionGroup.venue,
            room: showingOption.room,
            time: showingOption.time,
          };
      }
    }

    // not found, so return 'not found'
    return "Showing Not Found";

  }

  // update availability and allocations for showing
  async updateAvailability(bookingId: number) {
    var booking = this.bookings.find(f => f.id == bookingId);

    if (booking) {
      var allocations = await this.adminService.getShowingAllocations(booking.showingId);

      // initialise array
      this.availability = [];
      this.allocations = [];

      if (!allocations) {
        return;
      }

      // get rows/columns, and create list of available locations
      var room = this.showings.map(f => {
        var roomInner = f.showingOptions.find(g => g.id == booking.showingId);
        if (roomInner)
          return roomInner;
      });

      if (room.length == 0) {
        return;
      }

      // iterate rows/columns
      for (var i = 0; i < room[0].rows; i++) {
        for (var j = 0; j < room[0].columns; j++) {
          // get linear location value
          var locationValue = ((room[0].columns) * i) + j;
          // get location in hexavegesmal/numeral form
          var locationName = this.calculateLocationName(i, j);
          // if available, add to available, if not add to allocated
          if (allocations.indexOf(locationValue) == -1) {
            this.availability.push({ location: locationName, value: locationValue });
          } else {
            this.allocations.push({ location: locationName, value: locationValue });
          }

        }
      }

    }
  }

  private calculateLocationName(row: number, column: number): string {
    // columns are hexavigesimal, rows are base 10
    var hexvig = column + 1;
    var hexvigOutput = "";
    while (Math.floor(hexvig) > 0) {
      // needed to ensure mod output is zero based
      hexvig -= 1;
      // character represented by current position ('A' dec '65')
      hexvigOutput = String.fromCharCode(65 + (hexvig % 26)) + hexvigOutput;
      // move to next position
      hexvig /= 26;
    }

    return `col: ${hexvigOutput} - row: ${row}`;
  }

  
}
