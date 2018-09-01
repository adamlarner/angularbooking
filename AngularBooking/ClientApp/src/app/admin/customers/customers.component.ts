import { Component, OnInit } from '@angular/core';
import { FormGroup, AbstractControl, FormControl, Validators } from '@angular/forms';
import { AdminService } from '../_services/admin.service';
import { Customer } from '../../core/_models/entity/customer';
import { MessageService } from 'primeng/components/common/messageservice';

@Component({
  selector: 'app-customers',
  templateUrl: './customers.component.html',
  styleUrls: ['./customers.component.css']
})
export class CustomersComponent implements OnInit {

  displayDialog: boolean;
  newCustomer: boolean;

  selectedCustomer: Customer;
  customerForm: FormGroup;

  customers: Customer[] = [];

  cols: any[];

  // getters for shorter form control paths
  get id() { return this.customerForm.get("id"); }
  get firstName() { return this.customerForm.get("firstName"); }
  get lastName() { return this.customerForm.get("lastName"); }
  get address1() { return this.customerForm.get("address1"); }
  get address2() { return this.customerForm.get("address2"); }
  get address3() { return this.customerForm.get("address3"); }
  get address4() { return this.customerForm.get("address4"); }
  get address5() { return this.customerForm.get("address5"); }
  get contactPhone() { return this.customerForm.get("contactPhone"); }
  get contactEmail() { return this.customerForm.get("contactEmail"); }

  constructor(private adminService: AdminService, private messageService: MessageService) { }

  ngOnInit() {
    // configure add/edit form
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
        Validators.required
      ])
    },
      this.uniqueEmailValidator);

    // get customer data from service
    this.adminService.getCustomers("").then((e => {
      this.customers = e;
    }));

    // set up column defs for table
    this.cols = [
      { field: 'contactEmail', header: "Email" },
      { field: 'lastName', header: "Last Name" },
      { field: 'firstName', header: "First Name" }
    ];
  }

  showDialogToAdd() {
    this.selectedCustomer = null;
    this.newCustomer = true;
    this.customerForm.reset({ 'id': 0 });
    this.displayDialog = true;
  }

  async save(): Promise<void> {

    // check form data is valid
    if (!this.customerForm.valid)
      return;

    let customers = [...this.customers];

    // create new customer from form data
    let customer = new Customer(
      this.customerForm.controls.id.value,
      this.customerForm.controls.firstName.value,
      this.customerForm.controls.lastName.value,
      this.customerForm.controls.address1.value,
      this.customerForm.controls.address2.value,
      this.customerForm.controls.address3.value,
      this.customerForm.controls.address4.value,
      this.customerForm.controls.address5.value,
      this.customerForm.controls.contactPhone.value,
      this.customerForm.controls.contactEmail.value,
    );

    if (this.newCustomer) {
      // attempt post to service, then push to array on success
      var postSuccess = await this.adminService.createCustomer(customer);
      if (postSuccess) {
        customers.push(postSuccess);
        this.messageService.add({ severity: "success", summary: "Customer Created!", life: 2500 });
      } else {
        // display error message (todo: pretty)
        alert("error!");
      }
    }
    else {
      // attempt put to service
      var putSuccess = await this.adminService.updateCustomer(customer);
      if (putSuccess) {
        customers[this.customers.indexOf(this.selectedCustomer)] = customer;
        this.messageService.add({ severity: "success", summary: "Customer Modified!", life: 2500 });
      } else {
        // display error message (todo: pretty)
        alert("error!");
      }
    }

    this.customers = customers;
    this.displayDialog = false;
  }

  async delete() {
    if (!this.selectedCustomer)
      return;

    // attempt delete at service
    var deleteSuccess = await this.adminService.removeCustomer(this.selectedCustomer.id);

    if (deleteSuccess) {
      let index = this.customers.indexOf(this.selectedCustomer);
      this.customers = this.customers.filter((val, i) => i != index);
      this.displayDialog = false;
      this.messageService.add({ severity: "success", summary: "Customer Removed!", life: 2500 });
    }
  }

  onRowEdit(event) {
    this.selectedCustomer = event.data;
    this.newCustomer = false;
    this.customerForm.controls.id.setValue(event.data.id);
    this.customerForm.controls.firstName.setValue(event.data.firstName);
    this.customerForm.controls.lastName.setValue(event.data.lastName);
    this.customerForm.controls.address1.setValue(event.data.address1);
    this.customerForm.controls.address2.setValue(event.data.address2);
    this.customerForm.controls.address3.setValue(event.data.address3);
    this.customerForm.controls.address4.setValue(event.data.address4);
    this.customerForm.controls.address5.setValue(event.data.address5);
    this.customerForm.controls.contactPhone.setValue(event.data.contactPhone);
    this.customerForm.controls.contactEmail.setValue(event.data.contactEmail);
    
    this.displayDialog = true;

    return false;
  }

  // custom validator
  private uniqueEmailValidator = (control: AbstractControl) => {
    let id = control.get("id").value;
    let email = control.get("contactEmail").value;

    let found = this.customers.find((e) => {
      return e.contactEmail == email && e.id != id;
    });

    if (found) {
      control.get("contactEmail").setErrors({
        "notUnique": true
      });
      return {
        invalid: true
      }
    }
  }


}
