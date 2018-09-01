import { Component, OnInit } from '@angular/core';
import { Customer } from '../../core/_models/entity/customer';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { SiteService } from '../../site/_services/site.service';
import { NgbActiveModal, NgbPaginationConfig } from '@ng-bootstrap/ng-bootstrap';
import { Booking } from '../../core/_models/entity/booking';
import { ShowingOptionGroup } from '../../site/_models/showing-option-group';
import { ShowingOption } from '../../site/_models/showing-option';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {

  profileForm: FormGroup;

  // getters for shorter form control paths
  get id() { return this.profileForm.get("id"); }
  get firstName() { return this.profileForm.get("firstName"); }
  get lastName() { return this.profileForm.get("lastName"); }
  get address1() { return this.profileForm.get("address1"); }
  get address2() { return this.profileForm.get("address2"); }
  get address3() { return this.profileForm.get("address3"); }
  get address4() { return this.profileForm.get("address4"); }
  get address5() { return this.profileForm.get("address5"); }
  get contactPhone() { return this.profileForm.get("contactPhone"); }
  get contactEmail() { return this.profileForm.get("contactEmail"); }

  // booking information (all, and filtered)
  bookings: Booking[];
  filteredBookings: Booking[];

  // showings
  showingsOptions: ShowingOptionGroup[];

  // pagination controls for bookings
  page: number = 1;

  constructor(private siteService: SiteService, private activeModal: NgbActiveModal, private paginatorConfig: NgbPaginationConfig) {
    paginatorConfig.size = "sm";
    paginatorConfig.pageSize = 3;
  }

  ngOnInit() {
    // note: most of these fields will be required at booking, so defer check until then
    // set up profile form
    this.profileForm = new FormGroup({
      'id': new FormControl(0, [
        Validators.required
      ]),
      'firstName': new FormControl("", []),
      'lastName': new FormControl("", []),
      'address1': new FormControl("", []),
      'address2': new FormControl("", []),
      'address3': new FormControl("", []),
      'address4': new FormControl("", []),
      'address5': new FormControl("", []),
      'contactPhone': new FormControl("", []),
      'contactEmail': new FormControl("", []),
    });

    // pull profile information from service
    this.siteService.getProfile().then(profile => {
      this.profileForm.setValue({
        "id": profile.id,
        "firstName": profile.firstName,
        "lastName": profile.lastName,
        "address1": profile.address1,
        "address2": profile.address2,
        "address3": profile.address3,
        "address4": profile.address4,
        "address5": profile.address5,
        "contactPhone": profile.contactPhone,
        "contactEmail": profile.contactEmail
      });
    });

    // get showing options, which provide additional showing information for bookings
    this.siteService.getShowingOptions().then(showingOptions => {
      this.showingsOptions = showingOptions;
      // pull bookings for current profile from service
      this.siteService.getBookings(true).then(bookings => {
        this.bookings = bookings;
        this.bookingPageChanged(this.page);
      });
    });
    
  }

  closeModal(): boolean {
    this.activeModal.close();
    return false;
  }

  // updates profile information at service
  async update(): Promise<void> {

    // check form data is valid
    if (!this.profileForm.valid)
      return;

    // build new profile (customer) from form data
    let profile = new Customer(
      1,
      this.profileForm.controls.firstName.value,
      this.profileForm.controls.lastName.value,
      this.profileForm.controls.address1.value,
      this.profileForm.controls.address2.value,
      this.profileForm.controls.address3.value,
      this.profileForm.controls.address4.value,
      this.profileForm.controls.address5.value,
      this.profileForm.controls.contactPhone.value,
      this.profileForm.controls.contactEmail.value,
    );

    // attempt put to service
    var putSuccess = await this.siteService.updateProfile(profile);
    if (!putSuccess) {
      // display error message (todo: pretty)
      alert("error!");
    }

  }

  bookingPageChanged(pageNumber: number) {
    // create filtered array based upon page number
    var rangeStart = (pageNumber - 1) * this.paginatorConfig.pageSize;
    var rangeEnd = rangeStart + this.paginatorConfig.pageSize;
    this.filteredBookings = this.bookings.filter((f, index) => {
      return index >= rangeStart && index < rangeEnd;
    });
    
  }

  // return flat object containing showing details
  getShowingInfo(showingId: number) {
    var showingInfos = this.showingsOptions.map(f => {
      var showingOption = f.showingOptions.find(g => {
        return g.id == showingId;
      });
      
      if (showingOption) {
        return {
          event: f.event,
          venue: f.venue,
          room: showingOption.room,
          time: showingOption.time
        };
      }

      return null;

    }).filter(f => f != null);

    console.log(showingInfos);

    if (showingInfos.length == 1) {
      return showingInfos[0];
    }

    return {
      event: "no details",
      venue: "no details",
      room: "no details",
      time: "no details",
    }  
  }

  // cancel booking
  cancelBooking(id: number) {
    this.siteService.cancelBooking(id).then(f => {
      // todo - display confirmation
      this.siteService.getBookings(true).then(bookings => {
        this.bookings = bookings;
        this.bookingPageChanged(this.page);
      });
    });

    return false;
  }

}
