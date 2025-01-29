import { waitForAsync, ComponentFixture, TestBed } from '@angular/core/testing';

import { SearchExperimentComponent } from './search-experiment.component';

describe('SearchExperimentComponent', () => {
  let component: SearchExperimentComponent;
  let fixture: ComponentFixture<SearchExperimentComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ SearchExperimentComponent ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SearchExperimentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
