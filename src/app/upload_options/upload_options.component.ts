import { ViewChild } from "@angular/core";
import { MatMenuTrigger } from "@angular/material/menu";

class MyComponent {
    @ViewChild(MatMenuTrigger) trigger?: MatMenuTrigger;

    someMethod() {
        if (this.trigger) {
            this.trigger.openMenu();
        }
    }
}
