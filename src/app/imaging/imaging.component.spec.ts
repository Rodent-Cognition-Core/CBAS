import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ImagingComponent } from './imaging.component';

describe('ImagingComponent', () => {
  let component: ImagingComponent;
  let fixture: ComponentFixture<ImagingComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ImagingComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ImagingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
