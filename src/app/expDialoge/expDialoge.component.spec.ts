import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ExpDialogeComponent } from './expDialoge.component';

describe('ExpDialogeComponent', () => {
  let component: ExpDialogeComponent;
  let fixture: ComponentFixture<ExpDialogeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ExpDialogeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ExpDialogeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
