import { waitForAsync, ComponentFixture, TestBed } from '@angular/core/testing';

import { PubScreenComponent } from './pubScreen.component';

describe('PubScreenComponent', () => {
  let component: PubScreenComponent;
  let fixture: ComponentFixture<PubScreenComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [PubScreenComponent ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PubScreenComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
