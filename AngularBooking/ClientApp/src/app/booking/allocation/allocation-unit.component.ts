import { Component, OnInit, Input, HostBinding } from '@angular/core';

@Component({
  selector: 'app-allocation-unit',
  templateUrl: './allocation-unit.component.html',
  styleUrls: ['./allocation-unit.component.css']
})
export class AllocationUnitComponent implements OnInit {
  // determines whether allocation is already assigned
  @Input() available: boolean;
  // width/height % of allocation
  @Input() size: number;
  // determines whether allocation is selected in view
  @Input() selected: number;
  // colour of selected allocation
  @Input() colour: string;

  constructor() { }

  ngOnInit() {

  }
  
}
