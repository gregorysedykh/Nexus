import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute, convertToParamMap, provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { ApiService } from '../../services/api.service';

import { Home } from './home';

describe('Home', () => {
  let component: Home;
  let fixture: ComponentFixture<Home>;
  const apiServiceStub: Partial<ApiService> = {
    getUserWords: () => of([]),
    createWord: () => of({ id: 1, term: 'sample', languageCode: 'de' }),
    addWordToUser: () => of(void 0),
  };
  const activatedRouteStub = {
    snapshot: {
      queryParamMap: convertToParamMap({ userId: '1' }),
    },
  };

  beforeEach(async () => {
    sessionStorage.clear();

    await TestBed.configureTestingModule({
      imports: [Home],
      providers: [
        provideRouter([]),
        { provide: ActivatedRoute, useValue: activatedRouteStub },
        { provide: ApiService, useValue: apiServiceStub },
      ],
    })
    .compileComponents();

    fixture = TestBed.createComponent(Home);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
