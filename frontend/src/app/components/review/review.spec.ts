import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute, convertToParamMap, provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { ApiService } from '../../services/api.service';

import { Review } from './review';

describe('Review', () => {
  let component: Review;
  let fixture: ComponentFixture<Review>;
  const apiServiceStub: Partial<ApiService> = {
    getUserWords: () => of([]),
  };
  const activatedRouteStub = {
    snapshot: {
      queryParamMap: convertToParamMap({ userId: '1' }),
    },
  };

  beforeEach(async () => {
    sessionStorage.clear();

    await TestBed.configureTestingModule({
      imports: [Review],
      providers: [
        provideRouter([]),
        { provide: ActivatedRoute, useValue: activatedRouteStub },
        { provide: ApiService, useValue: apiServiceStub },
      ],
    })
    .compileComponents();

    fixture = TestBed.createComponent(Review);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
