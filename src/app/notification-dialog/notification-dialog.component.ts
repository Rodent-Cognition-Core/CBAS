import { Component, Input, Inject } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
    selector: 'message-dialog',
    templateUrl: './notification-dialog.component.html',
})
export class NotificationDialogComponent {

    public message: string;
    constructor(public dialogRef: MatDialogRef<NotificationDialogComponent>,
                @Inject(MAT_DIALOG_DATA) public data: any) {
        this.message = '';
    }

    //copyToClipboard() {

    //    navigator.clipboard.writeText(this.message);


    //}
    copyMessage(val: string) {
        const selBox = document.createElement('textarea');
        selBox.style.position = 'fixed';
        selBox.style.left = '0';
        selBox.style.top = '0';
        selBox.style.opacity = '0';
        selBox.value = val;
        document.body.appendChild(selBox);
        selBox.focus();
        selBox.select();
        document.execCommand('copy');
        document.body.removeChild(selBox);
    }
}
