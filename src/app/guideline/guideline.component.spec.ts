import { waitForAsync, ComponentFixture, TestBed } from '@angular/core/testing';

import { GuidelineComponent } from './guideline.component';

describe('GuidelineComponent', () => {
  let component: GuidelineComponent;
  let fixture: ComponentFixture<GuidelineComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ GuidelineComponent ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(GuidelineComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
