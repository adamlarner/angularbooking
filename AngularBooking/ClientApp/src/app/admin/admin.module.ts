import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminRoutingModule } from './/admin-routing.module';
import { DashboardComponent } from './dashboard/dashboard.component';
import { NavbarComponent } from './navbar/navbar.component';
import { AdminComponent } from './admin.component';
import { HttpClient, HttpClientModule, HttpClientXsrfModule } from '@angular/common/http';
import { SharedModule } from '../shared/shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgbModule, NgbDropdown } from '@ng-bootstrap/ng-bootstrap';
import { EventsComponent } from './events/events.component';
import { VenuesComponent } from './venues/venues.component';
import { CustomersComponent } from './customers/customers.component';
import { PricingComponent } from './pricing/pricing.component';
import { BookingsComponent } from './bookings/bookings.component';
import { TableModule } from 'primeng/table';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { DropdownModule } from 'primeng/dropdown';
import { FileUploadModule } from 'primeng/fileupload';
import { MultiSelectModule } from 'primeng/multiselect';
import { FieldsetModule } from 'primeng/fieldset';
import { CalendarModule } from 'primeng/calendar';
import { RoomsComponent } from './rooms/rooms.component';
import { RoomLayoutComponent } from './rooms/room-layout.component';
import { ShowingComponent } from './showing/showing.component';
import { ToastModule } from 'primeng/toast';
import { FeaturesComponent } from './features/features.component';

@NgModule({
  imports: [
    CommonModule,
    AdminRoutingModule,
    SharedModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    HttpClientXsrfModule,
    NgbModule,
    ToastModule,
    TableModule,
    DialogModule,
    ButtonModule,
    InputTextModule,
    InputTextareaModule,
    DropdownModule,
    FileUploadModule,
    MultiSelectModule,
    CalendarModule
  ],
  declarations: [DashboardComponent, NavbarComponent, AdminComponent, EventsComponent, VenuesComponent, CustomersComponent, PricingComponent, BookingsComponent, RoomsComponent, RoomLayoutComponent, ShowingComponent, FeaturesComponent],
  providers: [NgbDropdown]
})
export class AdminModule { }
