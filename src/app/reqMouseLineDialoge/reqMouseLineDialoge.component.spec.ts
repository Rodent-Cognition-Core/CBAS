import { waitForAsync, ComponentFixture, TestBed } from '@angular/core/testing';

import { ReqMouseLineDialogeComponent } from './reqMouseLineDialoge.component';

describe('ReqMouseLineDialogeComponent', () => {
  let component: ReqMouseLineDialogeComponent;
  let fixture: ComponentFixture<ReqMouseLineDialogeComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ReqMouseLineDialogeComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ReqMouseLineDialogeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
