import { Component, OnInit } from '@angular/core';

declare var $: any;

@Component({
    selector: 'app-guideline',
    templateUrl: './guideline.component.html',
    styleUrls: ['./guideline.component.scss']
})
export class GuidelineComponent implements OnInit {

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
