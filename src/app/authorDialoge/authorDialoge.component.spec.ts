import { waitforAsync, ComponentFixture, TestBed } from '@angular/core/testing';

import { AuthorDialogeComponent } from './authorDialoge.component';

describe('AuthorDialogeComponent', () => {
    let component: AuthorDialogeComponent;
    let fixture: ComponentFixture<AuthorDialogeComponent>;

  beforeEach(waitforAsync(() => {
    TestBed.configureTestingModule({
        declarations: [AuthorDialogeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
      fixture = TestBed.createComponent(AuthorDialogeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
