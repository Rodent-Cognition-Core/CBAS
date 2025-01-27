import { Component, Input, Inject } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-confirm-dialog',
  templateUrl: './delete-confirm-dialog.component.html',
})
export class DeleteConfirmDialogComponent {

  public confirmMessage: string;
  constructor(public dialogRef: MatDialogRef<DeleteConfirmDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    this.confirmMessage = '';
  }
}
