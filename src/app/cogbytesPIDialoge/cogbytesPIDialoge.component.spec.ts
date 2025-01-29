import { waitForAsync, ComponentFixture, TestBed } from '@angular/core/testing';

import { CogbytesPIDialogeComponent } from './cogbytesPIDialoge.component';

describe('CogbytesPIDialogeComponent', () => {
  let component: CogbytesPIDialogeComponent;
  let fixture: ComponentFixture<CogbytesPIDialogeComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [CogbytesPIDialogeComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CogbytesPIDialogeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
