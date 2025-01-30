import { NgModule } from '@angular/core';
import { Routes, RouterModule, PreloadAllModules } from '@angular/router';

import { HomeComponent } from './home/home.component';
// import { DataLinkComponent } from './data-link/data-link.component';


// We use PathLocationStrategy - the default "HTML 5 pushState" style:
// - https://angular.io/docs/ts/latest/guide/router.html#!#browser-url-styles
// - Router on the server (see Startup.cs) must match the router on the client to use PathLocationStrategy
// and Lazy Loading Modules:
// - https://angular.io/guide/ngmodule#lazy-loading-modules-with-the-router
const routes: Routes = [
  { path: '', redirectTo: 'home', pathMatch: 'full' },
  { path: 'home', component: HomeComponent },
  { path: 'account', loadChildren: () => import('./account/account.module').then(m => m.AccountModule) },
  { path: 'resources', loadChildren: () => import('./resources/resources.module').then(m => m.ResourcesModule) },
  { path: 'dashboard', loadChildren: () => import('./dashboard/dashboard.module').then(m => m.DashboardModule) },
  { path: 'taskAnalysis', loadChildren: () => import('./taskAnalysis/taskAnalysis.module').then(m => m.TaskAnalysisModule) },
  { path: 'upload', loadChildren: () => import('./upload/upload.module').then(m => m.UploadModule) },
  { path: 'animal-info', loadChildren: () => import('./animal-info/animal-info.module').then(m => m.AnimalInfoModule) },
  { path: 'experiment', loadChildren: () => import('./experiment/experiment.module').then(m => m.ExperimentModule) },
  { path: 'data-extraction', loadChildren: () => import('./data-extraction/data-extraction.module').then(m => m.DataExtractionModule) },
  { path: 'data-link', loadChildren: () => import('./data-link/data-link.module').then(m => m.DataLinkModule) },
  {
    path: 'search-experiment', loadChildren: () =>
      import('./search-experiment/search-experiment.module').then(m => m.SearchExperimentModule)
  },
  { path: 'manage-user', loadChildren: () => import('./manage-user/manage-user.module').then(m => m.ManageUserModule) },
  { path: 'profile', loadChildren: () => import('./profile/profile.module').then(m => m.ProfileModule) },
  { path: 'guideline', loadChildren: () => import('./guideline/guideline.module').then(m => m.GuidelineModule) },
  {
    path: 'guideline-datalab', loadChildren: () =>
      import('./guidelineDataLab/guidelineDataLab.module').then(m => m.GuidelineDataLabModule)
  },
  {
    path: 'data-visualization', loadChildren: () =>
      import('./data-visualization/data-visualization.module').then(m => m.DataVisualizationModule)
  },
  { path: 'mb-dashboard', loadChildren: () => import('./mb-dashboard/mb-dashboard.module').then(m => m.MBDashboardModule) },
  {
    path: 'pubScreen-dashboard', loadChildren: () =>
      import('./pubScreen-dashboard/pubScreen-dashboard.module').then(m => m.PSDashboardModule)
  },
  { path: 'contact-us', loadChildren: () => import('./contact-us/contact-us.module').then(m => m.ContactUsModule) },
  { path: 'download-ds', loadChildren: () => import('./download-ds/download-ds.module').then(m => m.DownloadDsModule) },
  { path: 'forms', loadChildren: () => import('./forms/forms.module').then(m => m.FormsModule) },
  { path: 'terms', loadChildren: () => import('./terms/terms.module').then(m => m.TermsModule) },
  { path: 'video-tutorial', loadChildren: () => import('./video-tutorial/video-tutorial.module').then(m => m.VideoTutorialModule) },
  { path: 'pubScreen', loadChildren: () => import('./pubScreen/pubScreen.module').then(m => m.PubScreenModule) },
  {
    path: 'pubScreen-search', loadChildren: () =>
      import('./pubScreen-search/pubScreen-search.module').then(m => m.PubScreenSearchModule)
  },
  { path: 'comp', loadChildren: () => import('./cogbytes/cogbytes.module').then(m => m.CogbytesModule) },
  { path: 'comp-search', loadChildren: () => import('./cogbytesSearch/cogbytesSearch.module').then(m => m.CogbytesSearchModule) },
  { path: 'pubScreen-queue', loadChildren: () => import('./pubScreenQueue/pubScreenQueue.module').then(m => m.PubScreenQueueModule) },
  { path: 'pubScreen-edit', loadChildren: () => import('./pubScreenEdit/pubScreenEdit.module').then(m => m.PubScreenEditModule) },
  { path: 'comp-edit', loadChildren: () => import('./cogbytesEdit/cogbytesEdit.module').then(m => m.CogbytesEditModule) },

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
