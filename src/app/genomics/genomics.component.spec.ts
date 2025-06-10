import { waitforAsync, ComponentFixture, TestBed } from '@angular/core/testing';

import { GenomicsComponent } from './genomics.component';

describe('GenomicsComponent', () => {
  let component: GenomicsComponent;
  let fixture: ComponentFixture<GenomicsComponent>;

  beforeEach(waitforAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ GenomicsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(GenomicsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
