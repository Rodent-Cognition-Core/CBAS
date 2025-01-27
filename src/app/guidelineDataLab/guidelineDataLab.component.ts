import { Component, OnInit, HostListener } from '@angular/core';

declare let $: any;

@Component({
  selector: 'app-guidelineDataLab',
  templateUrl: './guidelineDataLab.component.html',
  styleUrls: ['./guidelineDataLab.component.scss']
})
export class GuidelineDataLabComponent implements OnInit {

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
