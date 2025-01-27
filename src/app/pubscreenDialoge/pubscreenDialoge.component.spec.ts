import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PubscreenDialogeComponent } from './pubscreenDialoge.component';

describe('PubscreenDialogeComponent', () => {
  let component: PubscreenDialogeComponent;
  let fixture: ComponentFixture<PubscreenDialogeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [PubscreenDialogeComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PubscreenDialogeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
