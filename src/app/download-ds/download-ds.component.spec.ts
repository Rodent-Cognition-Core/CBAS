import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DownloadDsComponent } from './download-ds.component';

describe('DownloadDsComponent', () => {
  let component: DownloadDsComponent;
  let fixture: ComponentFixture<DownloadDsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DownloadDsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DownloadDsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
