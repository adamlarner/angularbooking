import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ShowingListComponent } from './showing-list.component';
import { FormsModule } from '@angular/forms';
import { NgbModal, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ShowingListEntryComponent } from './showing-list-entry.component';
import { ShowingListEntry } from '../_models/showing-list-entry';
import { SiteService } from '../_services/site.service';
import { HttpClient, HttpHandler } from '@angular/common/http';

describe('ShowingListComponent', () => {
  let component: ShowingListComponent;
  let fixture: ComponentFixture<ShowingListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ShowingListComponent, ShowingListEntryComponent],
      imports: [FormsModule, NgbModule.forRoot()],
      providers: [SiteService, HttpClient, HttpHandler]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ShowingListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
