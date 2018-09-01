import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormControl, Validators, AbstractControl } from '@angular/forms';
import { AdminService } from '../_services/admin.service';
import { Room } from '../../core/_models/entity/room';
import { Venue } from '../../core/_models/entity/venue';
import { MessageService } from 'primeng/components/common/messageservice';

@Component({
  selector: 'app-rooms',
  templateUrl: './rooms.component.html',
  styleUrls: ['./rooms.component.css']
})
export class RoomsComponent implements OnInit {

  displayDialog: boolean;
  newRoom: boolean;

  selectedRoom: Room;
  roomForm: FormGroup;

  rooms: Room[] = [];
  availableVenues: Venue[] = [];

  cols: any[];

  // getters for shorter form control paths
  get id() { return this.roomForm.get("id"); }
  get venueId() { return this.roomForm.get("venueId"); }
  get name() { return this.roomForm.get("name"); }
  get description() { return this.roomForm.get("description"); }

  get rows() { return this.roomForm.get("rows"); }
  get columns() { return this.roomForm.get("columns"); }
  get isles() { return this.roomForm.get("isles"); }

  // getters/setters for room layout component binding
  get rowsValue() { return this.roomForm.get("rows").value; }
  get columnsValue() { return this.roomForm.get("columns").value; }
  get islesValue() { return this.roomForm.get("isles").value; }

  set rowsValue(value) { this.roomForm.patchValue({ rows: value }) }
  set columnsValue(value) { this.roomForm.patchValue({ columns: value }) }
  set islesValue(value) { this.roomForm.patchValue({ isles: value }) }

  // validation group checks
  get basicValid() {
    return this.roomForm.get("venueId").valid
      && this.roomForm.get("name").valid
      && this.roomForm.get("description").valid
  }

  get layoutValid() {
    return this.roomForm.get("rows").valid
      && this.roomForm.get("columns").valid
      && this.roomForm.get("isles").valid
  }

  constructor(private adminService: AdminService, private messageService: MessageService) { }

  ngOnInit() {
    // configure add/edit form
    this.roomForm = new FormGroup({
      'id': new FormControl(0, [
        Validators.required
      ]),
      'venueId': new FormControl(0, [
        Validators.required
      ]),
      'name': new FormControl("", [
        Validators.required
      ]),
      'description': new FormControl("", [
        Validators.required
      ]),
      'rows': new FormControl(0, [
        Validators.required
      ]),
      'columns': new FormControl(0, [
        Validators.required
      ]),
      'isles': new FormControl("{ \"rows\": [], \"columns\": [] }", [
        Validators.required
      ])
    },
    this.uniqueNameValidator);

    // get available venues from service
    this.adminService.getVenues("?$select=Id,Name").then(e => {
      this.availableVenues = e;
    });

    // get event data from service
    this.adminService.getRooms("").then(e => {
      this.rooms = e;
    });

    // set up column defs for table
    this.cols = [
      { field: 'name', header: "Name" },
    ];
  }

  showDialogToAdd() {
    this.selectedRoom = null;
    this.newRoom = true;
    this.roomForm.reset({ 'id': 0 });
    this.roomForm.controls.isles.setValue("{ \"rows\":[], \"columns\":[] }");
    this.roomForm.controls.rows.setValue(5);
    this.roomForm.controls.columns.setValue(5);
    this.displayDialog = true;
  }

  async save(): Promise<void> {

    // check form data is valid
    if (!this.roomForm.valid)
      return;

    let rooms = [...this.rooms];

    // create new room from form data
    let room = new Room(
      this.roomForm.controls.id.value,
      this.roomForm.controls.venueId.value,
      this.roomForm.controls.name.value,
      this.roomForm.controls.description.value,
      this.roomForm.controls.rows.value,
      this.roomForm.controls.columns.value,
      this.roomForm.controls.isles.value
    );

    if (this.newRoom) {
      // attempt post to service, then push to array on success
      var postSuccess = await this.adminService.createRoom(room);
      if (postSuccess) {
        rooms.push(postSuccess);
        this.messageService.add({ severity: "success", summary: "Room Created!", life: 2500 });
      } else {
        // display error message (todo: pretty)
        alert("error!");
      }
    }
    else {
      // attempt put to service
      var putSuccess = await this.adminService.updateRoom(room);
      if (putSuccess) {
        rooms[this.rooms.indexOf(this.selectedRoom)] = room;
        this.messageService.add({ severity: "success", summary: "Room Modified!", life: 2500 });
      } else {
        // display error message (todo: pretty)
        alert("error!");
      }
    }

    this.rooms = rooms;
    this.displayDialog = false;
  }

  async delete() {
    if (!this.selectedRoom)
      return;

    // attempt delete at service
    var deleteSuccess = await this.adminService.removeRoom(this.selectedRoom.id);

    if (deleteSuccess) {
      let index = this.rooms.indexOf(this.selectedRoom);
      this.rooms = this.rooms.filter((val, i) => i != index);
      this.displayDialog = false;
      this.messageService.add({ severity: "success", summary: "Room Removed!", life: 2500 });
    }
  }

  onRowEdit(event) {
    this.selectedRoom = event.data;
    this.newRoom = false;
    this.roomForm.controls.id.setValue(event.data.id);
    this.roomForm.controls.venueId.setValue(event.data.venueId);
    this.roomForm.controls.name.setValue(event.data.name);
    this.roomForm.controls.description.setValue(event.data.description);
    this.roomForm.controls.rows.setValue(event.data.rows);
    this.roomForm.controls.columns.setValue(event.data.columns);
    this.roomForm.controls.isles.setValue(event.data.isles);
    this.displayDialog = true;

    return false;
  }

  venueNameLookup(id: number) {
    var found = this.availableVenues.find((v) => {
      return v.id == id;
    });
    if (found)
      return found.name;

    return "";
  }

  // custom validator (room name unique to venue)
  private uniqueNameValidator = (control: AbstractControl) => {
    let id = control.get("id").value;
    let venueId = control.get("venueId").value;
    let name = control.get("name").value;

    let found = this.rooms.find((e) => {
      return e.name == name && e.id != id && e.venueId == venueId;
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
