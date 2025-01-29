import { Component } from '@angular/core';

@Component({
    selector: 'app-loading-spinner',
    template: `
    <div class="loading-spinner">
      <div class="spinner"></div>
    </div>
  `,
    styleUrls: ['./loading-spinner.component.scss']
})
export class LoadingSpinnerComponent { }
