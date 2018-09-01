import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AllocationComponent } from './allocation.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AllocationUnitComponent } from './allocation-unit.component';

describe('AllocationComponent', () => {
  let component: AllocationComponent;
  let fixture: ComponentFixture<AllocationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [AllocationComponent, AllocationUnitComponent],
      imports: [
        FormsModule,
        ReactiveFormsModule
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AllocationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
