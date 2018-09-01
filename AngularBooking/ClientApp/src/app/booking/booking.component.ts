import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { MenuItem } from 'primeng/components/common/menuitem';
import { ShowingOption } from '../site/_models/showing-option';
import { ShowingOptionGroup } from '../site/_models/showing-option-group';
import { BookingService } from './_services/booking.service';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { SiteService } from '../site/_services/site.service';
import { AuthService } from '../core/_services/auth.service';
import { Customer } from '../core/_models/entity/customer';
import { MessageService } from 'primeng/components/common/messageservice';
import { LoginComponent } from '../shared/login/login.component';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Ng4LoadingSpinnerService } from 'ng4-loading-spinner';

@Component({
  selector: 'app-booking',
  templateUrl: './booking.component.html',
  styleUrls: ['./booking.component.css'],
  encapsulation: ViewEncapsulation.None
})
export class BookingComponent implements OnInit {

  // used to provide feedback to user is logged in
  loggedIn: boolean = false;

  // steps variables
  activeIndex: number = 0;
  items: MenuItem[];

  // user information
  customerForm: FormGroup;

  // booking information
  bookingAllocations: any;

  // showing information
  showingId: number;
  showingInfo: any;

  // allocation info
  allocationInfo: any;

  // progress flags/toggles
  isBooking: boolean;

  constructor(
    private bookingService: BookingService,
    private siteService: SiteService,
    private authService: AuthService,
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private messageService: MessageService,
    private modalService: NgbModal,
    private spinnerService: Ng4LoadingSpinnerService
  ) { }

  ngOnInit() {
    // initialise form group inputs
    this.initIdentityStep();

    // initialise steps items
    this.items = [
      { label: "Identity" },
      { label: "Seating" },
      { label: "Payment" },
      { label: "Confirm" }
    ];

    // get showing id from route
    this.activatedRoute.params.subscribe(params => {
      this.showingId = params["id"];

      // get allocation data
      this.bookingService.getAllocationData(this.showingId).then(allocations => {
        this.allocationInfo = allocations;
      });
      
      // get showing information
      this.bookingService.getShowingData(this.showingId).then(showing => {
        this.showingInfo = showing;
      });


    })

  }

  /*
   * Step Initializations
   */

  initIdentityStep() {

    this.customerForm = new FormGroup({
      'id': new FormControl(0, [
        Validators.required
      ]),
      'firstName': new FormControl("", [
        Validators.required
      ]),
      'lastName': new FormControl("", [
        Validators.required
      ]),
      'address1': new FormControl("", [
        Validators.required
      ]),
      'address2': new FormControl("", [
        Validators.required
      ]),
      'address3': new FormControl("", [
        Validators.required
      ]),
      'address4': new FormControl("", [
        Validators.required
      ]),
      'address5': new FormControl("", [
        Validators.required
      ]),
      'contactPhone': new FormControl("", [
        Validators.required
      ]),
      'contactEmail': new FormControl("", [
        Validators.required,
        Validators.email
      ]),
    });

    // if logged in, pull profile information from service
    if (this.authService.loggedIn.value) {
      this.siteService.getProfile().then(profile => {
        this.customerForm.setValue({
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
        this.loggedIn = true;
        if (this.loggedIn)
          this.customerForm.get("contactEmail").disable();
      });
    } else {
      this.customerForm.controls.id.setValue(0);
    }

  }

  // login to account
  login() {
    const modalRef = this.modalService.open(LoginComponent, {
      centered: true
    });
    let componentInstance: LoginComponent = modalRef.componentInstance;
    componentInstance.loginCallback = async (email, password) => {
      try {
        this.spinnerService.show();
        var loginResponse = await this.authService.login({ email, password });
        this.spinnerService.hide();

        if (loginResponse.status == "ok") {
          // re-initiate identity step setup
          this.initIdentityStep();
        } else {
          this.messageService.add({ severity: 'error', summary: "Login Failed", detail: "Incorrect Password", closable: true, life: 2500 });
        }

        return loginResponse;
      } catch (e) {
        this.spinnerService.hide();
      }
      
    }

    let modalObj = (modalRef as any);
    let modalWindow = modalObj._windowCmptRef.instance;
    setTimeout(() => {
      modalWindow.windowClass = 'modal-show'
    }, 50);

    let closeFunc = modalObj._removeModalElements.bind(modalRef);
    modalObj._removeModalElements = () => {
      modalWindow.windowClass = '';
      setTimeout(closeFunc, 250);
    };

    return false;
  }

  // validation checks
  allocationsValid(): boolean {
    // confirm that there is at least one location allocated
    return this.bookingAllocations && this.bookingAllocations.items.length > 0;
  }

  // component callbacks
  updateBookingAllocations(data) {
    this.bookingAllocations = data;
  }

  async confirmBooking(): Promise<void> {
    // confirm all fields are valid
    if (!this.customerForm || !this.allocationsValid())
      return;

    // build booking response
    var customer = new Customer(
      this.customerForm.get("id").value,
      this.customerForm.get("firstName").value,
      this.customerForm.get("lastName").value,
      this.customerForm.get("address1").value,
      this.customerForm.get("address2").value,
      this.customerForm.get("address3").value,
      this.customerForm.get("address4").value,
      this.customerForm.get("address5").value,
      this.customerForm.get("contactPhone").value,
      this.customerForm.get("contactEmail").value
    );

    var booking = {
      customer: customer,
      showingId: this.showingId,
      bookingItems: this.bookingAllocations.items
    }

    this.isBooking = true;
    try {
      this.spinnerService.show();
      var booked = await this.bookingService.createBooking(booking);

      if (booked) {
        // navigate to success page
        this.isBooking = false;
        this.spinnerService.hide();
        this.router.navigateByUrl("/booking/status");
      } else {
        // refresh allocations from service, and move back (chances are an allocation has been taken)
        this.bookingService.getAllocationData(this.showingId).then(allocations => {
          this.allocationInfo = allocations;
          this.isBooking = false;
          this.activeIndex = 1;
          this.messageService.add({ summary: "Booking failed: Please review available locations and try again", life: 3000, severity: "warning" });
          this.spinnerService.hide();
        }).catch(e => {
          this.isBooking = false;
          this.activeIndex = 1;
          this.messageService.add({ summary: "Booking failed: Please review available locations and try again", life: 3000, severity: "warning" });
          this.spinnerService.hide();
        });
      }

    } catch (e) {
      // refresh allocations from service, and move back (chances are an allocation has been taken)
      this.bookingService.getAllocationData(this.showingId).then(allocations => {
        this.allocationInfo = allocations;
        this.isBooking = false;
        this.activeIndex = 1;
        this.spinnerService.hide();
        this.messageService.add({ summary: "Booking failed: Please review available locations and try again", life: 3000, severity: "warning" });
      }).catch(e => {
        this.isBooking = false;
        this.activeIndex = 1;
        this.spinnerService.hide();
        this.messageService.add({ summary: "Booking failed: Please review available locations and try again", life: 3000, severity: "warning" });
      });
    }
    
  }

  

}
