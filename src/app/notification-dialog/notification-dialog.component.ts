import { Component, Input } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material';

@Component({
    selector: 'message-dialog',
    templateUrl: './notification-dialog.component.html',
})
export class NotificationDialogComponent {
    constructor(public dialogRef: MatDialogRef<NotificationDialogComponent>) { }

    public message: string;

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
