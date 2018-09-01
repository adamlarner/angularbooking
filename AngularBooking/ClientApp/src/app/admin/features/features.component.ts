import { Component, OnInit, ViewChild } from '@angular/core';
import { AdminService } from '../_services/admin.service';
import { FormGroup, FormControl, Validators, AbstractControl } from '@angular/forms';
import { FileUpload } from 'primeng/fileupload';
import { MessageService } from 'primeng/components/common/messageservice';
import { Feature } from '../../core/_models/entity/feature';

@Component({
  selector: 'app-features',
  templateUrl: './features.component.html',
  styleUrls: ['./features.component.css']
})
export class FeaturesComponent implements OnInit {

  @ViewChild("imageFileUpload") fileUploadComponent: FileUpload;

  displayDialog: boolean;
  newFeature: boolean;

  selectedFeature: Feature;
  featureForm: FormGroup;

  features: Feature[] = [];

  cols: any[];

  // getters for shorter form control paths
  get id() { return this.featureForm.get("id"); }
  get name() { return this.featureForm.get("name"); }
  get title() { return this.featureForm.get("title"); }
  get detail() { return this.featureForm.get("detail"); }
  get link() { return this.featureForm.get("link"); }
  get image() { return this.featureForm.get("image"); }
  get order() { return this.featureForm.get("order"); }

  constructor(private adminService: AdminService, private messageService: MessageService) { }

  ngOnInit() {
    // configure add/edit form
    this.featureForm = new FormGroup({
      'id': new FormControl(0, [
        Validators.required
      ]),
      'name': new FormControl("", [
        Validators.required
      ]),
      'title': new FormControl("", [
        Validators.required
      ]),
      'detail': new FormControl("", [
        Validators.required
      ]),
      'link': new FormControl("", [
        Validators.required
      ]),
      'image': new FormControl("", [
        Validators.required
      ]),
      'order': new FormControl(0, [
        Validators.required
      ])
    },
      this.uniqueNameValidator);

    // get feature data from service
    this.adminService.getFeatures("").then((e => {
      this.features = e;
    }));

    // set up column defs for table
    this.cols = [
      { field: 'name', header: "Name" },
      { field: 'title', header: "Title" },
      { field: 'order', header: "Order" }
    ];
  }

  showDialogToAdd() {
    this.selectedFeature = null;
    this.newFeature = true;
    this.featureForm.reset({ 'id': 0 });
    this.fileUploadComponent.clear();
    this.displayDialog = true;
  }

  async save(): Promise<void> {

    // check form data is valid
    if (!this.featureForm.valid)
      return;

    let features = [...this.features];

    // create new feature from form data
    let feature = new Feature(
      this.featureForm.controls.id.value,
      this.featureForm.controls.name.value,
      this.featureForm.controls.title.value,
      this.featureForm.controls.detail.value,
      this.featureForm.controls.link.value,
      this.featureForm.controls.image.value,
      this.featureForm.controls.order.value
    );

    if (this.newFeature) {
      // attempt post to service, then push to array on success
      var postSuccess = await this.adminService.createFeature(feature);
      if (postSuccess) {
        features.push(postSuccess);
        this.messageService.add({ severity: "success", summary: "Feature Created!", life: 2500 });
      } else {
        // display error message (todo: pretty)
        alert("error!");
      }
    }
    else {
      // attempt put to service
      var putSuccess = await this.adminService.updateFeature(feature);
      if (putSuccess) {
        features[this.features.indexOf(this.selectedFeature)] = feature;
        this.messageService.add({ severity: "success", summary: "Feature Modified!", life: 2500 });
      } else {
        // display error message (todo: pretty)
        alert("error!");
      }
    }

    this.features = features;
    this.displayDialog = false;
  }

  async delete() {
    if (!this.selectedFeature)
      return;

    // attempt delete at service
    var deleteSuccess = await this.adminService.removeFeature(this.selectedFeature.id);

    if (deleteSuccess) {
      let index = this.features.indexOf(this.selectedFeature);
      this.features = this.features.filter((val, i) => i != index);
      this.displayDialog = false;
      this.messageService.add({ severity: "success", summary: "Feature Removed!", life: 2500 });
    }
  }

  onRowEdit(event) {
    this.selectedFeature = event.data;
    this.newFeature = false;
    this.fileUploadComponent.clear();
    this.featureForm.controls.id.setValue(event.data.id);
    this.featureForm.controls.name.setValue(event.data.name);
    this.featureForm.controls.title.setValue(event.data.title);
    this.featureForm.controls.detail.setValue(event.data.detail);
    this.featureForm.controls.link.setValue(event.data.link);
    this.featureForm.controls.image.setValue(event.data.image);
    this.featureForm.controls.order.setValue(event.data.order);
    this.displayDialog = true;

    return false;
  }

  // select image file
  onImageFileSelect(event): void {
    if (event.files.length > 0) {
      // convert file to base64, and set within form data
      var fileReader = new FileReader();
      fileReader.onloadend = (file) => {
        this.featureForm.controls.image.setValue(fileReader.result);
      };
      fileReader.readAsDataURL(event.files[0]);
    }
  }

  // custom validator
  private uniqueNameValidator = (control: AbstractControl) => {
    let id = control.get("id").value;
    let name = control.get("name").value;

    let found = this.features.find((e) => {
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
