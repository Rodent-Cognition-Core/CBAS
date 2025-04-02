import { waitforAsync, ComponentFixture, TestBed } from '@angular/core/testing';

import { ReqPubModelDialogeComponent } from './reqPubModelDialoge.component';

describe('ReqGeneralDialogeComponent', () => {
    let component: ReqPubModelDialogeComponent;
    let fixture: ComponentFixture<ReqPubModelDialogeComponent>;

  beforeEach(waitforAsync(() => {
    TestBed.configureTestingModule({
        declarations: [ReqPubModelDialogeComponent]
    })
    .compileComponents();
  }));

  beforeEach(() => {
      fixture = TestBed.createComponent(ReqPubModelDialogeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
