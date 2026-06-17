import { TestBed } from '@angular/core/testing';

import { AppInitializerService } from './app-initializer';

describe('AppInitializerService', () => {
  let service: AppInitializerService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AppInitializerService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
