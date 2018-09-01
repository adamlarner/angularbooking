import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AllocationUnitComponent } from './allocation-unit.component';

describe('AllocationUnitComponent', () => {
  let component: AllocationUnitComponent;
  let fixture: ComponentFixture<AllocationUnitComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AllocationUnitComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AllocationUnitComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
