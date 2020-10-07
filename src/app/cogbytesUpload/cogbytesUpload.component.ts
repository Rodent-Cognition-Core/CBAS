import { Component, OnInit, Inject, NgModule } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FormControl, Validators, ReactiveFormsModule, FormGroup, FormBuilder } from '@angular/forms';
import { CogbytesDialogueComponent } from '../cogbytesDialogue/cogbytesDialogue.component'

@Component({

    selector: 'app-cogbytesUpload',
    templateUrl: './cogbytesUpload.component.html',
    styleUrls: ['./cogbytesUpload.component.scss'],
})

export class CogbytesUploadComponent implements OnInit {

    public uploadKey: number;
    public parentRef: CogbytesDialogueComponent;

    constructor() {

    }

    ngOnInit() {

    }

    remove() {
        this.parentRef.removeUploadComponent(this.uploadKey);
    }

}
