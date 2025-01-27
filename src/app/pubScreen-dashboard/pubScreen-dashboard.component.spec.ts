import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PSDashboardComponent } from './pubScreen-dashboard.component';

describe('DataVisualizationComponent', () => {
  let component: PSDashboardComponent;
  let fixture: ComponentFixture<PSDashboardComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [PSDashboardComponent ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PSDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
