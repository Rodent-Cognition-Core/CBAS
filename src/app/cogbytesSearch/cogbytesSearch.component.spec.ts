import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CogbytesSearchComponent } from './cogbytesSearch.component';

describe('CogbytesSearchComponent', () => {
  let component: CogbytesSearchComponent;
  let fixture: ComponentFixture<CogbytesSearchComponent>;

  beforeEach(async(() => {
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
