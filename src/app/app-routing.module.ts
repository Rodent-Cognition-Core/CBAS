import { NgModule } from '@angular/core';
import { Routes, RouterModule, PreloadAllModules } from '@angular/router';

import { HomeComponent } from './home/home.component';
//import { DataLinkComponent } from './data-link/data-link.component';


// We use PathLocationStrategy - the default "HTML 5 pushState" style:
// - https://angular.io/docs/ts/latest/guide/router.html#!#browser-url-styles
// - Router on the server (see Startup.cs) must match the router on the client to use PathLocationStrategy
// and Lazy Loading Modules:
// - https://angular.io/guide/ngmodule#lazy-loading-modules-with-the-router
const routes: Routes = [
    { path: '', redirectTo: 'home', pathMatch: 'full' },
    { path: 'home', component: HomeComponent },
    { path: 'account', loadChildren: './account/account.module#AccountModule' },
    { path: 'resources', loadChildren: './resources/resources.module#ResourcesModule' },
    { path: 'dashboard', loadChildren: './dashboard/dashboard.module#DashboardModule' },
    { path: 'taskAnalysis', loadChildren: './taskAnalysis/taskAnalysis.module#TaskAnalysisModule' },
    { path: 'upload', loadChildren: './upload/upload.module#UploadModule' },
    { path: 'animal-info', loadChildren: './animal-info/animal-info.module#AnimalInfoModule' },
    { path: 'experiment', loadChildren: './experiment/experiment.module#ExperimentModule' },
    { path: 'data-extraction', loadChildren: './data-extraction/data-extraction.module#DataExtractionModule' },
    { path: 'data-link', loadChildren: './data-link/data-link.module#DataLinkModule' },
    { path: 'search-experiment', loadChildren: './search-experiment/search-experiment.module#SearchExperimentModule' },
    { path: 'manage-user', loadChildren: './manage-user/manage-user.module#ManageUserModule' },
    { path: 'profile', loadChildren: './profile/profile.module#ProfileModule' },
    { path: 'imaging', loadChildren: './imaging/imaging.module#ImagingModule' },
    { path: 'genomics', loadChildren: './genomics/genomics.module#GenomicsModule' },
    { path: 'guideline', loadChildren: './guideline/guideline.module#GuidelineModule' },
    { path: 'data-visualization', loadChildren: './data-visualization/data-visualization.module#DataVisualizationModule' },
    { path: 'mb-dashboard', loadChildren: './mb-dashboard/mb-dashboard.module#MBDashboardModule' },
    { path: 'contact-us', loadChildren: './contact-us/contact-us.module#ContactUsModule' },
    { path: 'forms', loadChildren: './forms/forms.module#FormsModule' },
    { path: 'terms', loadChildren: './terms/terms.module#TermsModule' },
    { path: 'video-tutorial', loadChildren: './video-tutorial/video-tutorial.module#VideoTutorialModule' },
    { path: 'pubScreen', loadChildren: './pubScreen/pubScreen.module#PubScreenModule' },
    { path: 'pubScreen-search', loadChildren: './pubScreen-search/pubScreen-search.module#PubScreenSearchModule' },
    { path: 'cogbytes', loadChildren: './cogbytes/cogbytes.module#CogbytesModule' },

];

@NgModule({
    imports: [
        RouterModule.forRoot(routes, {
            preloadingStrategy: PreloadAllModules
        })
    ],
    exports: [RouterModule]
})
export class AppRoutingModule { }
