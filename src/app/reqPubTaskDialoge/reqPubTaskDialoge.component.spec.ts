import { waitForAsync, ComponentFixture, TestBed } from '@angular/core/testing';

import { ReqPubTaskDialogeComponent } from './reqPubTaskDialoge.component';

describe('ReqGeneralDialogeComponent', () => {
  let component: ReqPubTaskDialogeComponent;
  let fixture: ComponentFixture<ReqPubTaskDialogeComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ReqPubTaskDialogeComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ReqPubTaskDialogeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
