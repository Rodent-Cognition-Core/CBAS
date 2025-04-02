import { waitforAsync, ComponentFixture, TestBed } from '@angular/core/testing';

import { GuidelineDataLabComponent } from './guidelineDataLab.component';

describe('GuidelineDataLabComponent', () => {
    let component: GuidelineDataLabComponent;
    let fixture: ComponentFixture<GuidelineDataLabComponent>;

  beforeEach(waitforAsync(() => {
    TestBed.configureTestingModule({
        declarations: [GuidelineDataLabComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
      fixture = TestBed.createComponent(GuidelineDataLabComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
