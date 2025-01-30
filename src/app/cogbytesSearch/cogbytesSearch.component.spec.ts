import { waitForAsync, ComponentFixture, TestBed } from '@angular/core/testing';

import { CogbytesSearchComponent } from './cogbytesSearch.component';

describe('CogbytesSearchComponent', () => {
  let component: CogbytesSearchComponent;
  let fixture: ComponentFixture<CogbytesSearchComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [CogbytesSearchComponent ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CogbytesSearchComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
