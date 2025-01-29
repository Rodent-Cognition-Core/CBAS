import { waitForAsync, ComponentFixture, TestBed } from '@angular/core/testing';

import { CogbytesComponent } from './cogbytes.component';

describe('CogbytesComponent', () => {
  let component: CogbytesComponent;
  let fixture: ComponentFixture<CogbytesComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [CogbytesComponent ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CogbytesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
