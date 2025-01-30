import { waitForAsync, ComponentFixture, TestBed } from '@angular/core/testing';

import { SharedPubscreenComponent } from './shared-pubscreen.component';

describe('PubScreenComponent', () => {
  let component: SharedPubscreenComponent;
  let fixture: ComponentFixture<SharedPubscreenComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [SharedPubscreenComponent ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SharedPubscreenComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
