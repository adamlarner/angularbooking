import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ShowingComponent } from './showing.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { TableModule } from 'primeng/table';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { CalendarModule } from 'primeng/calendar';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/components/common/messageservice';

describe('ShowingComponent', () => {
  let component: ShowingComponent;
  let fixture: ComponentFixture<ShowingComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ShowingComponent],
      imports: [
        BrowserAnimationsModule,
        FormsModule,
        ReactiveFormsModule,
        NgbModule.forRoot(),
        TableModule,
        DialogModule,
        ButtonModule,
        CalendarModule,
        HttpClientTestingModule,
        ToastModule
      ],
      providers: [
        MessageService
      ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ShowingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
