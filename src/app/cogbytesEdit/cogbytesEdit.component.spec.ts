import { waitForAsync, ComponentFixture, TestBed } from '@angular/core/testing';

import { CogbytesEditComponent } from './cogbytesEdit.component';

describe('VideoTutorialComponent', () => {
  let component: CogbytesEditComponent;
  let fixture: ComponentFixture<CogbytesEditComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [CogbytesEditComponent ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CogbytesEditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
