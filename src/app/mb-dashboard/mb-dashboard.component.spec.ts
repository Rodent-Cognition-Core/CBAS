import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MBDashboardComponent } from './mb-dashboard.component';

describe('DataVisualizationComponent', () => {
    let component: MBDashboardComponent;
    let fixture: ComponentFixture<MBDashboardComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
        declarations: [MBDashboardComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
      fixture = TestBed.createComponent(MBDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
