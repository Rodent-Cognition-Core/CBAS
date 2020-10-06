import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CogbytesDialogueComponent } from './cogbytesDialogue.component';

describe('CogbytesDialogueComponent', () => {
    let component: CogbytesDialogueComponent;
    let fixture: ComponentFixture<CogbytesDialogueComponent>;

    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [CogbytesDialogueComponent]
        })
            .compileComponents();
    }));

    beforeEach(() => {
        fixture = TestBed.createComponent(CogbytesDialogueComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
