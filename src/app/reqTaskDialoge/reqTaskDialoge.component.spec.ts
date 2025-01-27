import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ReqTaskDialogeComponent } from './reqTaskDialoge.component';

describe('ReqTaskDialogeComponent', () => {
  let component: ReqTaskDialogeComponent;
  let fixture: ComponentFixture<ReqTaskDialogeComponent>;

  beforeEach(async(() => {
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
