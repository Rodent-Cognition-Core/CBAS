import { waitforAsync, ComponentFixture, TestBed } from '@angular/core/testing';
import { TermsDialogeComponent } from './termsDialoge.component';

describe('TermsDialogeComponent', () => {
    let component: TermsDialogeComponent;
    let fixture: ComponentFixture<TermsDialogeComponent>;

  beforeEach(waitforAsync(() => {
    TestBed.configureTestingModule({
        declarations: [TermsDialogeComponent]
    })
    .compileComponents();
  }));

  beforeEach(() => {
      fixture = TestBed.createComponent(TermsDialogeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
