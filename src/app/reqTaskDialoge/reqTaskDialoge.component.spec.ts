import { waitForAsync, ComponentFixture, TestBed } from '@angular/core/testing';

import { ReqTaskDialogeComponent } from './reqTaskDialoge.component';

describe('ReqTaskDialogeComponent', () => {
  let component: ReqTaskDialogeComponent;
  let fixture: ComponentFixture<ReqTaskDialogeComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ReqTaskDialogeComponent ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ReqTaskDialogeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
