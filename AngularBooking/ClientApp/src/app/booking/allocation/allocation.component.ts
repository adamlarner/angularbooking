import { Component, OnInit, Input, Output } from '@angular/core';
import { EventEmitter } from '@angular/core';
import { PricingStrategy } from '../../core/_models/entity/pricing-strategy';
import { BookingItem } from '../../core/_models/entity/booking-item';

@Component({
  selector: 'app-allocation',
  templateUrl: './allocation.component.html',
  styleUrls: ['./allocation.component.css']
})
export class AllocationComponent implements OnInit {

  // private 'set' variable members
  private _allocations: number[];

  // input parameters
  @Input() columns: number;
  @Input() rows: number;
  @Input() isles: any;
  @Input() pricing: PricingStrategy;

  @Input() set allocations(value: number[]) {
    // update allocation array
    this._allocations = value

    this.allocationArray = [];
    if (this._allocations) {
      for (var i = 0; i < this.rows; i++) {
        var rowArray = [];
        for (var j = 0; j < this.columns; j++) {
          var available = this._allocations.indexOf((i * this.columns) + j) == -1;
          rowArray.push({ available: available, selected: 0, colour: "" });
        }
        this.allocationArray.push(rowArray);
      }
    }    

    // update size of child unit allocations
    var rowTotal = this.rows + this.isles.rows.length;
    var columnTotal = this.columns + this.isles.columns.length;
    this.size = rowTotal > columnTotal ? 100 / rowTotal : 100 / columnTotal;
  }

  // output event emitters
  @Output() onAllocationChange = new EventEmitter();

  // unit size modifiers
  @Output() size: number;

  // component members
  allocationArray: any[];

  // color highlighting for units
  private selectedHighlights = [
    'green',
    'yellow',
    'blue',
    'orange',
    'purple',
    'cyan',
    'magenta',
    'lime',
    'pink'
  ];

  constructor() { }

  ngOnInit() {
    // initialise and populate allocation array
    this.allocationArray = [];
    for (var i = 0; i < this.rows; i++) {
      var rowArray = [];
      for (var j = 0; j < this.columns; j++) {
        var available = this._allocations.indexOf((i * this.columns) + j) == -1;
        rowArray.push({ available: available, selected: 0, colour: "" });
      }
      this.allocationArray.push(rowArray);
    }

    // update size of child unit allocations
    var rowTotal = this.rows + this.isles.rows.length;
    var columnTotal = this.columns + this.isles.columns.length;
    this.size = rowTotal > columnTotal ? 100 / rowTotal: 100 / columnTotal;

  }

  // lookup whether isle should exist
  rowIsleExists(row): boolean {
    return this.isles.rows.indexOf(row) != -1;
  }

  columnIsleExists(column): boolean {
    return this.isles.columns.indexOf(column) != -1;
  }

  calculateBookingTotal(): number {
    var total = 0;
    for (var i = 0; i < this.allocationArray.length; i++) {
      for (var j = 0; j < this.allocationArray[i].length; j++) {
        var allocation = this.allocationArray[i][j];
        if (allocation.selected > 0) {
          // get pricing for allocation
          total += this.pricing.pricingStrategyItems[allocation.selected - 1].price;
        }
      }
    }
    return total;
  }

  // called when allocation unit is clicked
  onAllocationClick(row, column) {
    // toggle allocation
    if (this.allocationArray[row][column].available) {
      this.allocationArray[row][column].colour = this.selectedHighlights[this.allocationArray[row][column].selected];
      this.allocationArray[row][column].selected++;
    }
    if (this.allocationArray[row][column].selected > this.pricing.pricingStrategyItems.length) {
      this.allocationArray[row][column].colour = 'white';
      this.allocationArray[row][column].selected = 0;
    }

    // emit booking allocations
    this.allocationChange();
  }

  // when allocation changes, emit object containing allocation details
  private allocationChange() {
    // iterate allocations and create list of desired booking items
    var bookingItems: any = {
      items: [],
      groups: {},
      total: 0,
      pricingStrategyId: this.pricing.id
    };
    for (var i = 0; i < this.allocationArray.length; i++) {
      for (var j = 0; j < this.allocationArray[i].length; j++) {
        var allocation = this.allocationArray[i][j];
        if (allocation.selected > 0 && allocation.available) {
          var location = (i * this.columns) + j;
          var pricingStrategyItem = this.pricing.pricingStrategyItems[allocation.selected - 1];
          bookingItems.items.push({
            location: location,
            pricingStrategyItemId: pricingStrategyItem.id
          });
          // create group for storing information per pricing strategy item (used for billing)
          if (!bookingItems.groups[pricingStrategyItem.id.toString()]) {
            bookingItems.groups[pricingStrategyItem.id.toString()] = {
              name: pricingStrategyItem.name,
              quantity: 0,
              total: 0,
            };
          }

          bookingItems.groups[pricingStrategyItem.id.toString()].total += pricingStrategyItem.price;
          bookingItems.groups[pricingStrategyItem.id.toString()].quantity++;

          // increment overall total
          bookingItems.total += this.pricing.pricingStrategyItems[allocation.selected - 1].price;
        }
        
      }
    }

    this.onAllocationChange.emit(bookingItems);
  }

  

}
