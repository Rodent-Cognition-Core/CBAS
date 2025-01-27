import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CogbytesAuthorDialogueComponent } from './cogbytesAuthorDialogue.component';

describe('CogbytesAuthorDialogueComponent', () => {
  let component: CogbytesAuthorDialogueComponent;
  let fixture: ComponentFixture<CogbytesAuthorDialogueComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [CogbytesAuthorDialogueComponent ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CogbytesAuthorDialogueComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
