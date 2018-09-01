import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { BookingComponent } from './booking.component';
import { BookingStatusComponent } from './booking-status.component';

const routes: Routes = [
  {
    path: 'status',
    component: BookingStatusComponent
  },
  {
    path: ':id',
    component: BookingComponent
  }
]

@NgModule({
  imports: [
    RouterModule.forChild(routes)
  ],
  exports: [
    RouterModule
  ],
  declarations: []
})
export class BookingRoutingModule { }
