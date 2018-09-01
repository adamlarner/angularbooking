import { Component, OnInit, ViewChild } from '@angular/core';
import { AdminService } from '../_services/admin.service';
import { Event } from '../../core/_models/entity/event';
import { FormGroup, FormControl, Validators, AbstractControl } from '@angular/forms';
import { FileUpload } from 'primeng/fileupload';
import { MessageService } from 'primeng/components/common/messageservice';

@Component({
  selector: 'app-events',
  templateUrl: './events.component.html',
  styleUrls: ['./events.component.css']
})
export class EventsComponent implements OnInit {

  @ViewChild("imageFileUpload") fileUploadComponent: FileUpload;

  displayDialog: boolean;
  newEvent: boolean;

  selectedEvent: Event;
  eventForm: FormGroup;

  events: Event[] = [];

  ageRatingSelection: any[] = [
    { label: "BBFC - UC", value: 0 },
    { label: "BBFC - U", value: 1 },
    { label: "BBFC - PG", value: 2 },
    { label: "BBFC - 12A", value: 3 },
    { label: "BBFC - 12", value: 4 },
    { label: "BBFC - 15", value: 5 },
    { label: "BBFC - 18", value: 6 },
    { label: "BBFC - R18", value: 7 },
    { label: "BBFC - TBC", value: 8 },
    { label: "PEGI - Universal", value: 9 },
    { label: "PEGI - PG", value: 10 },
    { label: "PEGI - 12A", value: 11 },
    { label: "PEGI - 12", value: 12 },
    { label: "PEGI - 15", value: 13 },
    { label: "PEGI - 18", value: 14 },
    { label: "PEGI - R18", value: 15 }
  ];

  cols: any[];

  // getters for shorter form control paths
  get id() { return this.eventForm.get("id"); }
  get name() { return this.eventForm.get("name"); }
  get description() { return this.eventForm.get("description"); }
  get image() { return this.eventForm.get("image"); }
  get duration() { return this.eventForm.get("duration"); }
  get ageRating() { return this.eventForm.get("ageRating"); }

  constructor(private adminService: AdminService, private messageService: MessageService) { }

  ngOnInit() {
    // configure add/edit form
    this.eventForm = new FormGroup({
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
      'duration': new FormControl(0, [
        Validators.required
      ]),
      'ageRating': new FormControl(0, [
        Validators.required
      ]),
    },
    this.uniqueNameValidator);

    // get event data from service
    this.adminService.getEvents("").then((e => {
      this.events = e;
    }));

    // set up column defs for table
    this.cols = [
      { field: 'name', header: "Name" },
      { field: 'duration', header: "Duration (mins)" },
      { field: 'ageRating', header: "Age Rating" }
    ];
  }

  showDialogToAdd() {
    this.selectedEvent = null;
    this.newEvent = true;
    this.eventForm.reset({ 'id': 0 });
    this.fileUploadComponent.clear();
    this.displayDialog = true;
  }

  async save(): Promise<void> {

    // check form data is valid
    if (!this.eventForm.valid)
      return;

    let events = [...this.events];

    // create new event from form data
    let event = new Event(
      this.eventForm.controls.id.value,
      this.eventForm.controls.name.value,
      this.eventForm.controls.description.value,
      this.eventForm.controls.image.value,
      this.eventForm.controls.duration.value,
      this.eventForm.controls.ageRating.value
    );

    if (this.newEvent) {
      // attempt post to service, then push to array on success
      var postSuccess = await this.adminService.createEvent(event);
      if (postSuccess) {
        events.push(postSuccess);
        this.messageService.add({ severity: "success", summary: "Event Created!", life: 2500 });
      } else {
        // display error message (todo: pretty)
        alert("error!");
      }
    }
    else {
      // attempt put to service
      var putSuccess = await this.adminService.updateEvent(event);
      if (putSuccess) {
        events[this.events.indexOf(this.selectedEvent)] = event;
        this.messageService.add({ severity: "success", summary: "Event Modified!", life: 2500 });
      } else {
        // display error message (todo: pretty)
        alert("error!");
      }
    }

    this.events = events;
    this.displayDialog = false;
  }

  async delete() {
    if (!this.selectedEvent)
      return;

    // attempt delete at service
    var deleteSuccess = await this.adminService.removeEvent(this.selectedEvent.id);

    if (deleteSuccess) {
      let index = this.events.indexOf(this.selectedEvent);
      this.events = this.events.filter((val, i) => i != index);
      this.displayDialog = false;
      this.messageService.add({ severity: "success", summary: "Event Removed!", life: 2500 });
    }
  }

  onRowEdit(event) {
    this.selectedEvent = event.data;
    this.newEvent = false;
    this.fileUploadComponent.clear();
    this.eventForm.controls.id.setValue(event.data.id);
    this.eventForm.controls.name.setValue(event.data.name);
    this.eventForm.controls.description.setValue(event.data.description);
    this.eventForm.controls.image.setValue(event.data.image);
    this.eventForm.controls.duration.setValue(event.data.duration);
    this.eventForm.controls.ageRating.setValue(event.data.ageRating);
    this.displayDialog = true;

    return false;
  }

  // select image file
  onImageFileSelect(event): void {
    if (event.files.length > 0) {
      // convert file to base64, and set within form data
      var fileReader = new FileReader();
      fileReader.onloadend = (file) => {
        this.eventForm.controls.image.setValue(fileReader.result);
      };
      fileReader.readAsDataURL(event.files[0]);
    }
  }

  // custom validator
  private uniqueNameValidator = (control: AbstractControl) => {
    let id = control.get("id").value;
    let name = control.get("name").value;

    let found = this.events.find((e) => {
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
