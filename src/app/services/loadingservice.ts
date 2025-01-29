import { Injectable } from '@angular/core';
import { Overlay, OverlayRef } from '@angular/cdk/overlay';
import { ComponentPortal } from '@angular/cdk/portal';
import { LoadingSpinnerComponent } from '../loading-spinner/loading-spinner.component'

@Injectable({
    providedIn: 'root'
})
export class LoadingService {
    private overlayRef: OverlayRef | null = null;

    constructor(private overlay: Overlay) { }

    show() {
        if (!this.overlayRef) {
            this.overlayRef = this.overlay.create({
                hasBackdrop: true,
                backdropClass: 'cdk-overlay-dark-backdrop',
                positionStrategy: this.overlay.position().global().centerHorizontally().centerVertically()
            });
            this.overlayRef.attach(new ComponentPortal(LoadingSpinnerComponent));
        }
    }

    hide() {
        if (this.overlayRef) {
            this.overlayRef.detach();
            this.overlayRef = null;
        }
    }
}
