import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
    selector: 'app-message-dialog',
    templateUrl: './notification-dialog.component.html',
})
export class NotificationDialogComponent {

    public message: string;
    constructor(public dialogRef: MatDialogRef<NotificationDialogComponent>,
                @Inject(MAT_DIALOG_DATA) public data: any) {
        this.message = '';
    }

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
