import { NgModule } from '@angular/core';
import { MatSelectModule } from '@angular/material/select';
import { UploadRoutingModule } from './upload-routing.module';
import { SharedModule } from '../shared/shared.module';
import { UploadService } from '../services/upload.service';
import { UploadComponent } from './upload.component';
import { DropzoneModule } from 'ngx-dropzone-wrapper';
import { DROPZONE_CONFIG } from 'ngx-dropzone-wrapper';
import { MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button'
import { environment } from '../../environments/environment'; // This path always points to 'environment.ts'

var api_url = environment.APP_URL;
@NgModule({
    imports: [
        UploadRoutingModule,
        SharedModule,
        DropzoneModule,
        MatSelectModule,
        MatDialogModule,
        MatButtonModule,

        
    ],
    declarations: [
        UploadComponent,

    ],
    providers: [
        UploadService,
        {
            provide: DROPZONE_CONFIG,
            useValue: {
                // Change this to your upload POST address:
                url: api_url+ '/api/upload/UploadFiles',
                // maxFilesize: 50,
                acceptedFiles: '.xml',
                // headers: { 'Authorization': this.authenticationService.getAuthorizationHeader() },
                parallelUploads: 10000,
                uploadMultiple: true,
                autoProcessQueue: false
            }
        }
    ],
    bootstrap: [UploadComponent],
    //bootstrap: [Dialogcomponent]

})
export class UploadModule { }

