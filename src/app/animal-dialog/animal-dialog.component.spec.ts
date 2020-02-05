import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AnimalDialogComponent } from './animal-dialog.component';

describe('AnimalDialogComponent', () => {
  let component: AnimalDialogComponent;
  let fixture: ComponentFixture<AnimalDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AnimalDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AnimalDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
