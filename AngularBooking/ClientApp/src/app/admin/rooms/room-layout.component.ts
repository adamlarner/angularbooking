import { Component, OnInit, OnChanges, Input, Output, EventEmitter, SimpleChanges } from '@angular/core';

@Component({
  selector: 'app-room-layout',
  templateUrl: './room-layout.component.html',
  styleUrls: ['./room-layout.component.css']
})
export class RoomLayoutComponent implements OnInit, OnChanges {

  @Input() columns: number;
  @Output() columnsChange = new EventEmitter();

  @Input() rows: number;
  @Output() rowsChange = new EventEmitter();

  @Input() isles: string;
  @Output() islesChange = new EventEmitter();

  // array items used for ngFor
  rowArray: number[];
  columnArray: number[];

  // arrays used internally for isle data
  rowIsles: number[] = new Array();
  columnIsles: number[] = new Array();
  rowIslesValue: string = "";
  columnIslesValue: string = "";
  
  constructor() { }

  ngOnInit() {
    this.rowArray = new Array(this.rows);
    this.columnArray = new Array(this.columns);
  }

  ngOnChanges(changes: SimpleChanges): void {
    this.rowArray = new Array(this.rows);
    this.columnArray = new Array(this.columns);

    // create isle arrays from isles json data
    try {
      var islesArrayData = JSON.parse(this.isles);
      this.rowIsles = islesArrayData.rows;
      this.columnIsles = islesArrayData.columns;
      // create additional string values for input binding
      this.rowIslesValue = this.rowIsles.toString();
      this.columnIslesValue = this.columnIsles.toString();
    } catch {
      // can't parse, so initialise with defaults
      this.rowIsles = [];
      this.columnIsles = [];
    }
    
  }

  // change events for @output
  onColumnsChange(event) {
    if (event != null) {
      let cols = parseInt(event);
      if (!isNaN(cols) && cols > 0) {
        this.columnArray = new Array(cols);
        this.columnsChange.emit(cols);
      } else return false;
    }

  }

  onRowsChange(event) {
    if (event != null) {
      let rows = parseInt(event);
      if (!isNaN(rows) && rows > 0) {
        this.rowArray = new Array(rows);
        this.rowsChange.emit(rows);
      } else return false;
    }
  }

  // change events for local input binding
  onIslesRowChange(event) {    
    try {
      // convert isle data back to number array
      this.rowIsles = JSON.parse(`[ ${event} ]`);
      this.columnIsles = JSON.parse(`[ ${this.columnIslesValue} ]`);

      var islesData = JSON.stringify({
        "rows": this.rowIsles,
        "columns": this.columnIsles
      });
     
      this.islesChange.emit(islesData);
    } catch {
      
    }
    
  }

  onIslesColumnChange(event) {
    try {
      // convert isle data back to number array
      this.rowIsles = JSON.parse(`[ ${this.rowIslesValue} ]`);
      this.columnIsles = JSON.parse(`[ ${event} ]`);

      // convert isle arrays to json string data
      var islesData = JSON.stringify({
        "rows": this.rowIsles,
        "columns": this.columnIsles
      });
      
      this.islesChange.emit(islesData);
    } catch {
      
    }
  }

  // row/column isle checks
  isRowIsle(index: number): boolean {
    if (this.rowIsles.indexOf(index) != -1)
      return true;
    return false;
  }

  isColumnIsle(index: number): boolean {
    if (this.columnIsles.indexOf(index) != -1)
      return true;
    return false;
  }

  columnIsleCount(): number {
    if (this.columnIsles.length == 0)
      return 0;
    var count = 0;
    for (var i = 0; i < this.columnIsles.length; i++) {
      if (this.columnIsles[i] <= this.columns)
        count++;
    }
    return count;
  }

}
