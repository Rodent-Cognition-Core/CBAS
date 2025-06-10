import { Component, OnInit, AfterViewChecked } from '@angular/core';

@Component({
    selector: 'app-guideline',
    templateUrl: './guideline.component.html',
    styleUrls: ['./guideline.component.scss']
})
export class GuidelineComponent implements OnInit, AfterViewChecked {

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
