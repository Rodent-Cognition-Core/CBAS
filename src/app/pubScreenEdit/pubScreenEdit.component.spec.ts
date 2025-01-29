import { waitForAsync, ComponentFixture, TestBed } from '@angular/core/testing';

import { PubScreenEditComponent } from './pubScreenEdit.component';

describe('VideoTutorialComponent', () => {
  let component: PubScreenEditComponent;
  let fixture: ComponentFixture<PubScreenEditComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [PubScreenEditComponent ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PubScreenEditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
