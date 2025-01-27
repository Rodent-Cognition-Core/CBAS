import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DataLinkComponent } from './data-link.component';

describe('DataLinkComponent', () => {
  let component: DataLinkComponent;
  let fixture: ComponentFixture<DataLinkComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DataLinkComponent ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DataLinkComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
