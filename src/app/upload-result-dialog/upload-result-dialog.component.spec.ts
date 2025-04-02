import { waitforAsync, ComponentFixture, TestBed } from '@angular/core/testing';

import { UploadResultDialogComponent } from './upload-result-dialog.component';

describe('UploadResultDialogComponent', () => {
  let component: UploadResultDialogComponent;
  let fixture: ComponentFixture<UploadResultDialogComponent>;

  beforeEach(waitforAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ UploadResultDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UploadResultDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
