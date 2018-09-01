import { Component, OnInit } from '@angular/core';
import { FormGroup, Validators, FormControl, AbstractControl } from '@angular/forms';
import { AdminService } from '../_services/admin.service';
import { Showing } from '../../core/_models/entity/showing';
import { Event } from '../../core/_models/entity/event';
import { PricingStrategy } from '../../core/_models/entity/pricing-strategy';
import { Room } from '../../core/_models/entity/room';
import { Venue } from '../../core/_models/entity/venue';
import { MessageService } from 'primeng/components/common/messageservice';

@Component({
  selector: 'app-showing',
  templateUrl: './showing.component.html',
  styleUrls: ['./showing.component.css']
})
export class ShowingComponent implements OnInit {

  displayDialog: boolean;
  newShowing: boolean;

  selectedShowing: Showing;
  showingForm: FormGroup;

  showings: Showing[] = [];
  
  cols: any[];

  events: Event[];
  venues: Venue[];
  pricingStrategies: PricingStrategy[];

  // getters for shorter form control paths
  get id() { return this.showingForm.get("id"); }
  get eventId() { return this.showingForm.get("eventId"); }
  get roomId() { return this.showingForm.get("roomId"); }
  get startTime() { return this.showingForm.get("startTime"); }
  get endTime() { return this.showingForm.get("endTime"); }
  get pricingStrategyId() { return this.showingForm.get("pricingStrategyId"); }

  constructor(private adminService: AdminService, private messageService: MessageService) { }

  ngOnInit() {
    // configure add/edit form
    this.showingForm = new FormGroup({
      'id': new FormControl(0, [
        Validators.required
      ]),
      'eventId': new FormControl(0, [
        Validators.required
      ]),
      'roomId': new FormControl(0, [
        Validators.required
      ]),
      'startTime': new FormControl(null, [
        Validators.required
      ]),
      'endTime': new FormControl(null, [
        Validators.required
      ]),
      'pricingStrategyId': new FormControl(0, [
        Validators.required
      ]),
    },
      this.availableSlotValidator);

    // get showing data from service
    this.adminService.getShowings("").then(e => {
      this.showings = e;
    });

    // get additional related data from service (events, venues(with rooms), pricing strategies)
    this.adminService.getEvents("?$select=Id,Name").then(e => {
      this.events = e;
    });
    this.adminService.getVenues("?$expand=rooms&$select=Id,Name,Rooms").then(e => {
      this.venues = e;
    });
    this.adminService.getPricingStrategies("?$select=Id,Name").then(e => {
      this.pricingStrategies = e;
    });

    // set up column defs for table
    this.cols = [
      { field: 'eventId', header: "Event Id" },
      { field: 'roomId', header: "Room Id" },
    ];
  }

  showDialogToAdd() {
    this.selectedShowing = null;
    this.newShowing = true;
    this.showingForm.reset({ 'id': 0 });
    this.displayDialog = true;
  }

  async save(): Promise<void> {

    // check form data is valid
    if (!this.showingForm.valid)
      return;

    let showings = [...this.showings];

    // output date as ISO-8601 string
    let startTimeIso = this.showingForm.controls.startTime.value.toISOString();
    let endTimeIso = this.showingForm.controls.endTime.value.toISOString();

    // create new event from form data
    let showing = new Showing(
      this.showingForm.controls.id.value,
      this.showingForm.controls.eventId.value,
      this.showingForm.controls.roomId.value,
      startTimeIso,
      endTimeIso,
      this.showingForm.controls.pricingStrategyId.value
    );

    if (this.newShowing) {
      // attempt post to service, then push to array on success
      var postSuccess = await this.adminService.createShowing(showing);
      if (postSuccess) {
        showings.push(postSuccess);
        this.messageService.add({ severity: "success", summary: "Showing Created!", life: 2500 });
      } else {
        // display error message (todo: pretty)
        alert("error!");
      }
    }
    else {
      // attempt put to service
      var putSuccess = await this.adminService.updateShowing(showing);
      if (putSuccess) {
        showings[this.showings.indexOf(this.selectedShowing)] = showing;
        this.messageService.add({ severity: "success", summary: "Showing Modified!", life: 2500 });
      } else {
        // display error message (todo: pretty)
        alert("error!");
      }
    }

    this.showings = showings;
    this.displayDialog = false;
  }

  async delete() {
    if (!this.selectedShowing)
      return;

    // attempt delete at service
    var deleteSuccess = await this.adminService.removeShowing(this.selectedShowing.id);

    if (deleteSuccess) {
      let index = this.showings.indexOf(this.selectedShowing);
      this.showings = this.showings.filter((val, i) => i != index);
      this.displayDialog = false;
      this.messageService.add({ severity: "success", summary: "Showing Removed!", life: 2500 });
    }
  }

  onRowEdit(showing) {
    // format dates from ISO-8601 string to unix epoch
    let startDate: Date = new Date(showing.data.startTime);
    let endDate: Date = new Date(showing.data.endTime);

    this.selectedShowing = showing.data;
    this.newShowing = false;
    this.showingForm.controls.id.setValue(showing.data.id);
    this.showingForm.controls.eventId.setValue(showing.data.eventId);
    this.showingForm.controls.roomId.setValue(showing.data.roomId);
    this.showingForm.controls.pricingStrategyId.setValue(showing.data.pricingStrategyId);;
    this.showingForm.controls.startTime.setValue(startDate);
    this.showingForm.controls.endTime.setValue(endDate);
    this.displayDialog = true;

    return false;
  }

  // get event/venue for table display
  getEventName(id: number): string {
    if (!this.events)
      return "";

    let event = this.events.find((event) => {
      return event.id == id;
    });
    return event.name || "";
  }

  getVenueNameFromRoomId(id: number): string {
    if (!this.venues)
      return "";

    let venue = this.venues.find((venue) => {
      return venue.rooms.some((room) => {
        return room.id == id;
      });
    });
    return venue.name || "";
  }
  
  // custom validator
  private availableSlotValidator = (control: AbstractControl) => {
    let id = control.get("id").value;
    let eventId = control.get("eventId").value;
    let roomId = control.get("roomId").value;
    let pricingStrategyId = control.get("pricingStrategyId").value;
    let startTime = control.get("startTime").value;
    let endTime = control.get("endTime").value;

    let found = this.showings.find((e) => {
      // check initial criteria (same showing)
      if (e.id != id
       && e.eventId == eventId
        && e.roomId == roomId) {
        // convert string datetime to unix epoch
        var startTimeA: number = Date.parse(e.startTime);
        var endTimeA: number = Date.parse(e.endTime);
        var startTimeB: number = Date.parse(startTime);
        var endTimeB: number = Date.parse(endTime);

        // check for collision of times
        if ((startTimeA > startTimeB && startTimeA < endTimeB)
          || (endTimeA > startTimeB && endTimeA < endTimeB)
          || (startTimeB > startTimeA && startTimeB < endTimeA)
          || (endTimeB > startTimeA && endTimeB < endTimeA)
        ) {
          return true;
        }
      }
      return false;      
    });

    if (found) {
      control.setErrors({
        "alreadyInUse": true
      });
      return {
        invalid: true
      }
    }
  }
}
