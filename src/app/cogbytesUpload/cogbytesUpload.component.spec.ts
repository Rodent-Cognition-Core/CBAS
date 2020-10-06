import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CogbytesUploadComponent } from './cogbytesUpload.component';

describe('CogbytesUploadComponent', () => {
    let component: CogbytesUploadComponent;
    let fixture: ComponentFixture<CogbytesUploadComponent>;

    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [CogbytesUploadComponent]
        })
            .compileComponents();
    }));

    beforeEach(() => {
        fixture = TestBed.createComponent(CogbytesUploadComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
