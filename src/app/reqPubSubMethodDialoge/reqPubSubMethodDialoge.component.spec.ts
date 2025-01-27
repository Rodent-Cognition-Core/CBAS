import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ReqPubSubMethodDialogeComponent } from './reqPubSubMethodDialoge.component';

describe('ReqGeneralDialogeComponent', () => {
  let component: ReqPubSubMethodDialogeComponent;
  let fixture: ComponentFixture<ReqPubSubMethodDialogeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ReqPubSubMethodDialogeComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ReqPubSubMethodDialogeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
