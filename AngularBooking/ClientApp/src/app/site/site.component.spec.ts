import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';

import { SiteComponent } from './site.component';
import { NavbarComponent } from './navbar/navbar.component';
import { FeatureComponent } from './feature/feature.component';
import { ShowingListComponent } from './showing-list/showing-list.component';
import { EventDetailComponent } from './event-detail/event-detail.component';
import { VenueDetailComponent } from './venue-detail/venue-detail.component';
import { SiteDetailComponent } from './site-detail/site-detail.component';
import { NgbModule, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule } from '@angular/forms';
import { NgbModalStack } from '@ng-bootstrap/ng-bootstrap/modal/modal-stack';
import { HttpClient, HttpHandler } from '@angular/common/http';
import { SharedModule } from '../shared/shared.module';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { AuthService } from '../core/_services/auth.service';
import { CsrfResponse } from '../core/_models/csrf-response';
import { HttpResponse } from 'selenium-webdriver/http';
import { LoginUserResponse } from '../core/_models/login-user-response';
import { ShowingListEntryComponent } from './showing-list/showing-list-entry.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MessageService } from 'primeng/components/common/messageservice';
import { Ng4LoadingSpinnerModule } from 'ng4-loading-spinner';


describe('SiteComponent', () => {
  let component: SiteComponent;
  let fixture: ComponentFixture<SiteComponent>;

  let service: AuthService;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [SiteComponent, NavbarComponent, FeatureComponent, ShowingListComponent, EventDetailComponent, VenueDetailComponent, SiteDetailComponent, ShowingListEntryComponent],
      imports: [BrowserAnimationsModule, RouterTestingModule, HttpClientTestingModule, NgbModule.forRoot(), FormsModule, SharedModule, Ng4LoadingSpinnerModule.forRoot()],
      providers: [AuthService, MessageService]
    })
      .compileComponents();

    service = TestBed.get(AuthService);

  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SiteComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    var spy = spyOn(service, "updateCsrfToken").and.returnValue(new CsrfResponse("", ""));
    var spy = spyOn(service, "isAuth").and.returnValue(false);
    expect(component).toBeTruthy();
  });
});
