import { TestBed, inject } from '@angular/core/testing';

import { BookingService } from './booking.service';
import { HttpClientTestingModule } from '@angular/common/http/testing';

describe('BookingService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [
        HttpClientTestingModule
      ],
      providers: [BookingService]
    });
  });

  it('should be created', inject([BookingService], (service: BookingService) => {
    expect(service).toBeTruthy();
  }));
});
