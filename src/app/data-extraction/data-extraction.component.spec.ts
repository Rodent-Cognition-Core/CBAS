import { waitforAsync, ComponentFixture, TestBed } from '@angular/core/testing';

import { DataExtractionComponent } from './data-extraction.component';

describe('DataExtractionComponent', () => {
  let component: DataExtractionComponent;
  let fixture: ComponentFixture<DataExtractionComponent>;

  beforeEach(waitforAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ DataExtractionComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DataExtractionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
