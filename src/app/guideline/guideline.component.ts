import { Component, OnInit, HostListener, NgModule } from '@angular/core';

declare let $: any;

@Component({
  selector: 'app-guideline',
  templateUrl: './guideline.component.html',
  styleUrls: ['./guideline.component.scss']
})
export class GuidelineComponent implements OnInit {

  showItem = false;
  constructor() { }

  ngOnInit() {
    setTimeout(() => {
      this.showItem = false;
    });
  }

  ngAfterViewChecked() {
    setTimeout(() => {
      this.showItem = true;
    });
  }

}
