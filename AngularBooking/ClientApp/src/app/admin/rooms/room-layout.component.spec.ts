import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RoomLayoutComponent } from './room-layout.component';

describe('RoomLayoutComponent', () => {
  let component: RoomLayoutComponent;
  let fixture: ComponentFixture<RoomLayoutComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RoomLayoutComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RoomLayoutComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
