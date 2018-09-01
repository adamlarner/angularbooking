import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, Validators, FormControl, AbstractControl } from '@angular/forms';
import { Venue } from '../../core/_models/entity/venue';
import { AdminService } from '../_services/admin.service';
import { FacilityFlags } from '../../core/_models/entity/facility-flags';
import { FileUpload } from 'primeng/fileupload';
import { MessageService } from 'primeng/components/common/messageservice';

@Component({
  selector: 'app-venues',
  templateUrl: './venues.component.html',
  styleUrls: ['./venues.component.css']
})
export class VenuesComponent implements OnInit {

  @ViewChild("venueImageFileUpload") fileUploadComponent: FileUpload;

  displayDialog: boolean;
  newVenue: boolean;

  selectedVenue: Venue;
  venueForm: FormGroup;

  venues: Venue[] = [];

  displayRoomDialog: boolean;

  facilityOptions: any[] = [
    { label: "Toilets", value: 1 },
    { label: "Parking", value: 2 },
    { label: "Disabled Access", value: 4 },
    { label: "Audio Described", value: 8 },
    { label: "Guide Dogs Permitted", value: 16 },
    { label: "Subtitled", value: 32 },
    { label: "Bar", value: 64 }
  ];

  cols: any[];

  // getters for shorter form control paths
  get id() { return this.venueForm.get("id"); }
  get name() { return this.venueForm.get("name"); }
  get description() { return this.venueForm.get("description"); }
  get image() { return this.venueForm.get("image"); }
  get address1() { return this.venueForm.get("address1"); }
  get address2() { return this.venueForm.get("address2"); }
  get address3() { return this.venueForm.get("address3"); }
  get address4() { return this.venueForm.get("address4"); }
  get address5() { return this.venueForm.get("address5"); }
  get contactPhone() { return this.venueForm.get("contactPhone"); }
  get latLong() { return this.venueForm.get("latLong"); }
  get website() { return this.venueForm.get("website"); }
  get facebook() { return this.venueForm.get("facebook"); }
  get twitter() { return this.venueForm.get("twitter"); }
  get instagram() { return this.venueForm.get("instagram"); }
  get facilities() { return this.venueForm.get("facilities"); }

  // validation group checks
  get basicValid() {
    return this.venueForm.get("name").valid
      && this.venueForm.get("description").valid
      && this.venueForm.get("image").valid
      && this.venueForm.get("latLong").valid;
  }

  get addressValid() {
    return this.venueForm.get("address1").valid
      && this.venueForm.get("address2").valid
      && this.venueForm.get("address3").valid
      && this.venueForm.get("address4").valid
      && this.venueForm.get("address5").valid
      && this.venueForm.get("contactPhone").valid
      && this.venueForm.get("website").valid;
  }

  get socialValid() {
    return this.venueForm.get("facebook").valid
      && this.venueForm.get("twitter").valid
      && this.venueForm.get("instagram").valid;
  }

  constructor(private adminService: AdminService, private messageService: MessageService) { }

  ngOnInit() {
    // configure add/edit form
    this.venueForm = new FormGroup({
      'id': new FormControl(0, [
        Validators.required
      ]),
      'name': new FormControl("", [
        Validators.required
      ]),
      'description': new FormControl("", [
        Validators.required
      ]),
      'image': new FormControl("", [
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
      'latLong': new FormControl("", [
        Validators.required
      ]),
      'website': new FormControl("", []),
      'facebook': new FormControl("", []),
      'twitter': new FormControl("", []),
      'instagram': new FormControl("", []),
      'facilities': new FormControl([], []),
    },
    this.uniqueNameValidator);

    // get event data from service
    this.adminService.getVenues("").then((e => {
      this.venues = e;
    }));

    // set up column defs for table
    this.cols = [
      { field: 'name', header: "Name" },
      { field: 'contactPhone', header: "Contact Phone" }
    ];
  }

  showDialogToAdd() {
    this.selectedVenue = null;
    this.newVenue = true;
    this.venueForm.reset({ 'id': 0 });
    this.fileUploadComponent.clear();
    this.displayDialog = true;
  }

  async save(): Promise<void> {

    // check form data is valid
    if (!this.venueForm.valid)
      return;

    let venues = [...this.venues];

    // sum facilities array to get correct value for facility flag enum
    let facilityFlags: FacilityFlags = FacilityFlags.None;
    if (this.venueForm.controls.facilities.value)
      facilityFlags = this.venueForm.controls.facilities.value.reduce((a, b) => a + b, 0);

    // create new venue from form data
    let controlsRef = this.venueForm.controls;
    let venue = new Venue(
      controlsRef.id.value,
      controlsRef.name.value,
      controlsRef.description.value,
      controlsRef.image.value,
      controlsRef.address1.value,
      controlsRef.address2.value,
      controlsRef.address3.value,
      controlsRef.address4.value,
      controlsRef.address5.value,
      controlsRef.contactPhone.value,
      controlsRef.latLong.value,
      controlsRef.website.value,
      controlsRef.facebook.value,
      controlsRef.twitter.value,
      controlsRef.instagram.value,
      facilityFlags
    );

    if (this.newVenue) {
      // attempt post to service, then push to array on success
      var postSuccess = await this.adminService.createVenue(venue);
      if (postSuccess) {
        venues.push(postSuccess);
        this.messageService.add({ severity: "success", summary: "Venue Created!", life: 2500 });
      } else {
        // display error message (todo: pretty)
        alert("error!");
      }
    }
    else {
      // attempt put to service
      var putSuccess = await this.adminService.updateVenue(venue);
      if (putSuccess) {
        venues[this.venues.indexOf(this.selectedVenue)] = venue;
        this.messageService.add({ severity: "success", summary: "Venue Modified!", life: 2500 });
      } else {
        // display error message (todo: pretty)
        alert("error!");
      }
    }

    this.venues = venues;
    this.displayDialog = false;
  }

  async delete() {
    if (!this.selectedVenue)
      return;

    // attempt delete at service
    var deleteSuccess = await this.adminService.removeVenue(this.selectedVenue.id);

    if (deleteSuccess) {
      let index = this.venues.indexOf(this.selectedVenue);
      this.venues = this.venues.filter((val, i) => i != index);
      this.displayDialog = false;
      this.messageService.add({ severity: "success", summary: "Venue Removed!", life: 2500 });
    }
  }

  onRowEdit(event) {
    this.selectedVenue = event.data;
    this.newVenue = false;
    this.fileUploadComponent.clear();
    this.venueForm.controls.id.setValue(event.data.id);
    this.venueForm.controls.name.setValue(event.data.name);
    this.venueForm.controls.description.setValue(event.data.description);
    this.venueForm.controls.image.setValue(event.data.image);
    this.venueForm.controls.address1.setValue(event.data.address1);
    this.venueForm.controls.address2.setValue(event.data.address2);
    this.venueForm.controls.address3.setValue(event.data.address3);
    this.venueForm.controls.address4.setValue(event.data.address4);
    this.venueForm.controls.address5.setValue(event.data.address5);
    this.venueForm.controls.contactPhone.setValue(event.data.contactPhone);
    this.venueForm.controls.latLong.setValue(event.data.latLong);
    this.venueForm.controls.website.setValue(event.data.website);
    this.venueForm.controls.facebook.setValue(event.data.facebook);
    this.venueForm.controls.twitter.setValue(event.data.twitter);
    this.venueForm.controls.instagram.setValue(event.data.instagram);

    // create facilities list from enum
    let facilitiesList: number[] = [];
    for (let i = 0; i < 7; i++) {
      if (event.data.facilities & (1 << i))
        facilitiesList.push(1 << i);
    }
    this.venueForm.controls.facilities.setValue(facilitiesList);

    this.displayDialog = true;
  }

  onRowRooms(event) {
    this.displayRoomDialog = true;
  }

  // select image file
  onImageFileSelect(event): void {
    if (event.files.length > 0) {
      // convert file to base64, and set within form data
      var fileReader = new FileReader();
      fileReader.onloadend = (file) => {
        this.venueForm.controls.image.setValue(fileReader.result);
      };
      fileReader.readAsDataURL(event.files[0]);
    }
  }

  // custom validator
  private uniqueNameValidator = (control: AbstractControl) => {
    let id = control.get("id").value;
    let name = control.get("name").value;

    let found = this.venues.find((e) => {
      return e.name == name && e.id != id;
    });

    if (found) {
      control.get("name").setErrors({
        "notUnique": true
      });
      return {
        invalid: true
      }
    }

  }

}
