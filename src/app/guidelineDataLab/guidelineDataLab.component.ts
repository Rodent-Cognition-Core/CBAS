import { Component, OnInit, AfterViewChecked } from '@angular/core';

@Component({
    selector: 'app-guideline-data-lab',
    templateUrl: './guidelineDataLab.component.html',
    styleUrls: ['./guidelineDataLab.component.scss']
})
export class GuidelineDataLabComponent implements OnInit, AfterViewChecked {

    showItem: boolean = false;
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
