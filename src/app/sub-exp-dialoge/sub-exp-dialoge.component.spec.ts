import { waitforAsync, ComponentFixture, TestBed } from '@angular/core/testing';

import { SubExpDialogeComponent } from './sub-exp-dialoge.component';

describe('SubExpDialogeComponent', () => {
    let component: SubExpDialogeComponent;
    let fixture: ComponentFixture<SubExpDialogeComponent>;

    beforeEach(waitforAsync(() => {
        TestBed.configureTestingModule({
            declarations: [SubExpDialogeComponent]
        })
            .compileComponents();
    }));

    beforeEach(() => {
        fixture = TestBed.createComponent(SubExpDialogeComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
