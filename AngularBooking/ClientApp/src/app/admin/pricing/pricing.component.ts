import { Component, OnInit } from '@angular/core';
import { FormGroup, AbstractControl, FormControl, Validators } from '@angular/forms';
import { AdminService } from '../_services/admin.service';
import { PricingStrategy } from '../../core/_models/entity/pricing-strategy';
import { PricingStrategyItem } from '../../core/_models/entity/pricing-strategy-item';
import { MessageService } from 'primeng/components/common/messageservice';

@Component({
  selector: 'app-pricing',
  templateUrl: './pricing.component.html',
  styleUrls: ['./pricing.component.css']
})
export class PricingComponent implements OnInit {


  // common dialog members
  newPricing: boolean;
  pricingForm: FormGroup;
  
  // pricing strategy state members
  displayPricingStrategyDialog: boolean;
  selectedPricingStrategy: PricingStrategy;
  pricingStrategies: PricingStrategy[] = [];
  pricingStrategyCols: any[];

  // pricing strategy item state members
  displayPricingStrategyItemDialog: boolean;
  selectedPricingStrategyItem: PricingStrategyItem;
  pricingStrategyItems: PricingStrategyItem[] = [];
  selectedPricingStrategyId: number;

  // getters for shorter form control paths
  // used for both pricing strategy and items
  get id() { return this.pricingForm.get("id"); }
  get name() { return this.pricingForm.get("name"); }
  get description() { return this.pricingForm.get("description"); }
  get price() { return this.pricingForm.get("price"); }

  constructor(private adminService: AdminService, private messageService: MessageService) { }

  ngOnInit() {
    // configure add/edit form (pricing strategy)
    this.pricingForm = new FormGroup({
      'id': new FormControl(0, [
        Validators.required
      ]),
      'name': new FormControl("", [
        Validators.required
      ]),
      'description': new FormControl("", [
        Validators.required
      ]),
      'price': new FormControl(0, [
        Validators.required
      ])
    },
    this.uniquePricingNameValidator);

    // get pricing strategy data from service
    this.adminService.getPricingStrategies("").then((e => {
      this.pricingStrategies = e;
    }));

    // get pricing strategy data from service
    this.adminService.getPricingStrategyItems("").then((e => {
      this.pricingStrategyItems = e;
    }));

    // set up column defs for table
    this.pricingStrategyCols = [
      { field: 'name', header: "Name" }
    ];

  }

  /*
   * Pricing Strategy CRUD methods
   **/
  
  showDialogToAddPricingStrategy() {
    this.selectedPricingStrategy = null;
    this.newPricing = true;
    this.pricingForm.reset({ 'id': 0, 'price': 0 });
    this.displayPricingStrategyDialog = true;
  }

  async savePricingStrategy(): Promise<void> {

    // check form data is valid
    if (!this.pricingForm.valid)
      return;

    let pricingStrategies = [...this.pricingStrategies];

    // create new pricing strategy from form data
    let pricingStrategy = new PricingStrategy(
      this.pricingForm.controls.id.value,
      this.pricingForm.controls.name.value,
      this.pricingForm.controls.description.value
    );

    if (this.newPricing) {
      // attempt post to service, then push to array on success
      var postSuccess = await this.adminService.createPricingStrategy(pricingStrategy);
      if (postSuccess) {
        pricingStrategies.push(postSuccess);
        this.messageService.add({ severity: "success", summary: "Pricing Strategy Created!", life: 2500 });
      } else {
        // display error message (todo: pretty)
        alert("error!");
      }
    }
    else {
      // attempt put to service
      var putSuccess = await this.adminService.updatePricingStrategy(pricingStrategy);
      if (putSuccess) {
        pricingStrategies[this.pricingStrategies.indexOf(this.selectedPricingStrategy)] = pricingStrategy;
        this.messageService.add({ severity: "success", summary: "Pricing Strategy Modified!", life: 2500 });
      } else {
        // display error message (todo: pretty)
        alert("error!");
      }
    }

    this.pricingStrategies = pricingStrategies;
    this.displayPricingStrategyDialog = false;
  }

  async deletePricingStrategy() {
    if (!this.selectedPricingStrategy)
      return;

    // attempt delete at service
    var deleteSuccess = await this.adminService.removePricingStrategy(this.selectedPricingStrategy.id);

    if (deleteSuccess) {
      let index = this.pricingStrategies.indexOf(this.selectedPricingStrategy);
      this.pricingStrategies = this.pricingStrategies.filter((val, i) => i != index);
      this.displayPricingStrategyDialog = false;
      this.messageService.add({ severity: "success", summary: "Pricing Strategy Removed!", life: 2500 });
    }
  }

  onPricingStrategyRowEdit(pricingStrategy) {
    this.selectedPricingStrategy = pricingStrategy.data;
    this.newPricing = false;
    this.pricingForm.controls.id.setValue(pricingStrategy.data.id);
    this.pricingForm.controls.name.setValue(pricingStrategy.data.name);
    this.pricingForm.controls.description.setValue(pricingStrategy.data.description);
    this.displayPricingStrategyDialog = true;

    return false;
  }

  /*
   * Pricing Strategy Item CRUD methods
   **/

  showDialogToAddPricingStrategyItem(pricingStrategyId) {
    this.selectedPricingStrategyItem = null;
    this.newPricing = true;
    this.pricingForm.reset({ 'id': 0 });
    this.selectedPricingStrategyId = pricingStrategyId;
    this.displayPricingStrategyItemDialog = true;
  }

  async savePricingStrategyItem(): Promise<void> {

    // check form data is valid
    if (!this.pricingForm.valid)
      return;

    let pricingStrategyItems = [...this.pricingStrategyItems];

    // create new pricing strategy from form data
    let pricingStrategyItem = new PricingStrategyItem(
      this.pricingForm.controls.id.value,
      this.selectedPricingStrategyId,
      this.pricingForm.controls.name.value,
      this.pricingForm.controls.description.value,
      this.pricingForm.controls.price.value
    );

    if (this.newPricing) {
      // attempt post to service, then push to array on success
      var postSuccess = await this.adminService.createPricingStrategyItem(pricingStrategyItem);
      if (postSuccess) {
        pricingStrategyItems.push(postSuccess);
        this.messageService.add({ severity: "success", summary: "Pricing Strategy Item Created!", life: 2500 });
      } else {
        // display error message (todo: pretty)
        alert("error!");
      }
    }
    else {
      // attempt put to service
      var putSuccess = await this.adminService.updatePricingStrategyItem(pricingStrategyItem);
      if (putSuccess) {
        pricingStrategyItems[this.pricingStrategyItems.indexOf(this.selectedPricingStrategyItem)] = pricingStrategyItem;
        this.messageService.add({ severity: "success", summary: "Pricing Strategy Item Modified!", life: 2500 });
      } else {
        // display error message (todo: pretty)
        alert("error!");
      }
    }

    this.pricingStrategyItems = pricingStrategyItems;
    this.displayPricingStrategyItemDialog = false;
  }

  async deletePricingStrategyItem() {
    if (!this.selectedPricingStrategyItem)
      return;

    // attempt delete at service
    var deleteSuccess = await this.adminService.removePricingStrategyItem(this.selectedPricingStrategyItem.id);

    if (deleteSuccess) {
      let index = this.pricingStrategyItems.indexOf(this.selectedPricingStrategyItem);
      this.pricingStrategyItems = this.pricingStrategyItems.filter((val, i) => i != index);
      this.displayPricingStrategyItemDialog = false;
      this.messageService.add({ severity: "success", summary: "Pricing Strategy Item Removed!", life: 2500 });
    }
  }

  onPricingStrategyItemRowEdit(pricingStrategyItem, pricingStrategyId) {
    this.selectedPricingStrategyItem = pricingStrategyItem.data;
    this.selectedPricingStrategyId = pricingStrategyId;
    this.newPricing = false;
    this.pricingForm.controls.id.setValue(pricingStrategyItem.data.id);
    this.pricingForm.controls.name.setValue(pricingStrategyItem.data.name);
    this.pricingForm.controls.description.setValue(pricingStrategyItem.data.description);
    this.pricingForm.controls.price.setValue(pricingStrategyItem.data.price);
    this.displayPricingStrategyItemDialog = true;

    return false;
  }

  getPricingItemsByStrategyId(id) {
    return this.pricingStrategyItems.filter((item) => item.pricingStrategyId == id);
  }

  // custom validator
  private uniquePricingNameValidator = (control: AbstractControl) => {
    let id = control.get("id").value;
    let name = control.get("name").value;

    let found: boolean = false;

    if (this.displayPricingStrategyDialog) {
      var pricingStrategy = this.pricingStrategies.find((e) => {
        return e.name == name && e.id != id;
      });
      found = pricingStrategy != null;
    }
    if (this.displayPricingStrategyItemDialog) {
      var pricingStrategyItem = this.pricingStrategyItems.find((e) => {
        return e.name == name && e.id != id && e.pricingStrategyId == this.selectedPricingStrategyId;
      });
      found = pricingStrategyItem != null;
    }

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
