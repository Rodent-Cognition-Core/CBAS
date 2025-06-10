import { waitforAsync, ComponentFixture, TestBed } from '@angular/core/testing';

import { SharedExperimentComponent } from './shared-experiment.component';

describe('SharedExperimentComponent', () => {
  let component: SharedExperimentComponent;
  let fixture: ComponentFixture<SharedExperimentComponent>;

  beforeEach(waitforAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ SharedExperimentComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SharedExperimentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
