import { waitforAsync, ComponentFixture, TestBed } from '@angular/core/testing';

import { NotificationDialogComponent } from './notification-dialog.component';

describe('NotificationDialogComponent', () => {
    let component: NotificationDialogComponent;
    let fixture: ComponentFixture<NotificationDialogComponent>;

    beforeEach(waitforAsync(() => {
        TestBed.configureTestingModule({
            declarations: [NotificationDialogComponent]
        })
            .compileComponents();
    }));

    beforeEach(() => {
        fixture = TestBed.createComponent(NotificationDialogComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
