import { waitforAsync, ComponentFixture, TestBed } from '@angular/core/testing';

import { PubScreenSearchComponent } from './pubScreen-search.component';

describe('VideoTutorialComponent', () => {
    let component: PubScreenSearchComponent;
    let fixture: ComponentFixture<PubScreenSearchComponent>;

  beforeEach(waitforAsync(() => {
    TestBed.configureTestingModule({
        declarations: [PubScreenSearchComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
      fixture = TestBed.createComponent(PubScreenSearchComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
