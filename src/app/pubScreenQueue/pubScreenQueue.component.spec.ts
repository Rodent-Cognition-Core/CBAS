import { waitForAsync, ComponentFixture, TestBed } from '@angular/core/testing';

import { PubScreenQueueComponent } from './pubScreenQueue.component';

describe('PubScreenQueueComponent', () => {
  let component: PubScreenQueueComponent;
  let fixture: ComponentFixture<PubScreenQueueComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [PubScreenQueueComponent ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PubScreenQueueComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
