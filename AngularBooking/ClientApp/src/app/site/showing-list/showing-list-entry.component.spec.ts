import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ShowingListEntryComponent } from './showing-list-entry.component';
import { FormsModule } from '@angular/forms';
import { RouterTestingModule } from '@angular/router/testing';

describe('ShowingListEntryComponent', () => {
  let component: ShowingListEntryComponent;
  let fixture: ComponentFixture<ShowingListEntryComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ShowingListEntryComponent],
      imports: [
        FormsModule,
        RouterTestingModule
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ShowingListEntryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
