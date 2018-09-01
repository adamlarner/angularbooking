import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { ShowingListComponent } from './showing-list/showing-list.component';
import { EventDetailComponent } from './event-detail/event-detail.component';
import { VenueDetailComponent } from './venue-detail/venue-detail.component';
import { SiteDetailComponent } from './site-detail/site-detail.component';
import { SiteComponent } from './site.component';
import { VenueListComponent } from './venue-list/venue-list.component';

const routes: Routes = [
  {
    path: '',
    component: SiteComponent,
    children: [
      {
        path: '',
        redirectTo: 'listing',
        pathMatch: 'full'
      },
      {
        path: 'listing',
        component: ShowingListComponent,
        data: { state: 'listing' }
      },
      {
        path: 'venues',
        component: VenueListComponent,
        data: { state: 'venues' }
      },
      {
        path: 'event/:id',
        component: EventDetailComponent
      },
      {
        path: 'venue/:id',
        component: VenueDetailComponent
      },
      {
        path: 'contact',
        component: SiteDetailComponent,
        data: { state: 'contact' }
      }
    ]
  }

];

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
export class SiteRoutingModule { }
