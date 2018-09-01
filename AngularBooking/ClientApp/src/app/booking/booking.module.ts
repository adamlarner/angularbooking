import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BookingComponent } from './booking.component';
import { BookingRoutingModule } from './/booking-routing.module';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { StepsModule } from 'primeng/steps';

import { AllocationComponent } from './allocation/allocation.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { AllocationUnitComponent } from './allocation/allocation-unit.component';
import { BookingStatusComponent } from './booking-status.component';
import { ToastModule } from 'primeng/toast';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    BookingRoutingModule,
    NgbModule,
    StepsModule,
    ToastModule
  ],
  declarations: [BookingComponent, AllocationComponent, AllocationUnitComponent, BookingStatusComponent]
})
export class BookingModule { }
