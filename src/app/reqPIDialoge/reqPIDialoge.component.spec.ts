import { waitForAsync, ComponentFixture, TestBed } from '@angular/core/testing';

import { ReqPIDialogeComponent } from './reqPIDialoge.component';

describe('ReqPIDialogeComponent', () => {
  let component: ReqPIDialogeComponent;
  let fixture: ComponentFixture<ReqPIDialogeComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ReqPIDialogeComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ReqPIDialogeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
