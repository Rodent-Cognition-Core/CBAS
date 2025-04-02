import { waitforAsync, ComponentFixture, TestBed } from '@angular/core/testing';

import { ReqAgeDialogeComponent } from './reqAgeDialoge.component';

describe('ReqAgeDialogeComponent', () => {
    let component: ReqAgeDialogeComponent;
    let fixture: ComponentFixture<ReqAgeDialogeComponent>;

  beforeEach(waitforAsync(() => {
    TestBed.configureTestingModule({
        declarations: [ReqAgeDialogeComponent]
    })
    .compileComponents();
  }));

  beforeEach(() => {
      fixture = TestBed.createComponent(ReqAgeDialogeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
