import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { DashboardComponent } from './dashboard/dashboard.component';
import { AdminComponent } from './admin.component';
import { EventsComponent } from './events/events.component';
import { CustomersComponent } from './customers/customers.component';
import { VenuesComponent } from './venues/venues.component';
import { PricingComponent } from './pricing/pricing.component';
import { BookingsComponent } from './bookings/bookings.component';
import { RoomsComponent } from './rooms/rooms.component';
import { ShowingComponent } from './showing/showing.component';
import { FeaturesComponent } from './features/features.component';

const routes: Routes = [
  {
    path: '',
    component: AdminComponent,
    children: [
      {
        path: '',
        redirectTo: 'features',
        pathMatch: 'full'
      },
      {
        path: 'features',
        component: FeaturesComponent,
        data: { title: "Features" }
      },
      {
        path: 'customers',
        component: CustomersComponent,
        data: { title: "Customers" }
      },
      {
        path: 'events',
        component: EventsComponent,
        data: { title: "Events" }
      },
      {
        path: 'venues',
        component: VenuesComponent,
        data: { title: "Venues" }
      },
      {
        path: 'rooms',
        component: RoomsComponent,
        data: { title: "Rooms" }
      },
      {
        path: 'pricing',
        component: PricingComponent,
        data: { title: "Pricing" }
      },
      {
        path: 'bookings',
        component: BookingsComponent,
        data: { title: "Bookings" }
      },
      {
        path: 'showings',
        component: ShowingComponent,
        data: { title: "Showings" }
      }
    ]
  }
]

@NgModule({
  imports: [
    CommonModule,
    RouterModule.forChild(routes)
  ],
  exports: [
    RouterModule
  ],
  declarations: []
})
export class AdminRoutingModule { }
