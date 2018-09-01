import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SiteRoutingModule } from './/site-routing.module';
import { NavbarComponent } from './navbar/navbar.component';
import { FeatureComponent } from './feature/feature.component';
import { ShowingListComponent } from './showing-list/showing-list.component';
import { EventDetailComponent } from './event-detail/event-detail.component';
import { VenueDetailComponent } from './venue-detail/venue-detail.component';
import { SiteDetailComponent } from './site-detail/site-detail.component';
import { SiteComponent } from './site.component';

import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HttpClientXsrfModule } from '@angular/common/http';
import { SharedModule } from '../shared/shared.module';
import { ShowingListEntryComponent } from './showing-list/showing-list-entry.component';
import { VenueListComponent } from './venue-list/venue-list.component';

@NgModule({
  imports: [
    CommonModule,
    SiteRoutingModule,
    SharedModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    HttpClientXsrfModule,
    NgbModule
  ],
  declarations: [NavbarComponent, FeatureComponent, ShowingListComponent, EventDetailComponent, VenueDetailComponent, SiteDetailComponent, SiteComponent, ShowingListEntryComponent, VenueListComponent]
})
export class SiteModule { }
