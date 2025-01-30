import { waitForAsync, ComponentFixture, TestBed } from '@angular/core/testing';

import { ReqGeneralDialogeComponent } from './reqGeneralDialoge.component';

describe('ReqGeneralDialogeComponent', () => {
  let component: ReqGeneralDialogeComponent;
  let fixture: ComponentFixture<ReqGeneralDialogeComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ReqGeneralDialogeComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ReqGeneralDialogeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
