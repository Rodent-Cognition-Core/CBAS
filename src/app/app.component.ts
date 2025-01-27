import { Component, OnInit, Input, OnChanges, ElementRef, HostListener } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { Title } from '@angular/platform-browser';
import { Observable, of } from 'rxjs';
import { OAuthService } from 'angular-oauth2-oidc';
import { AuthenticationService } from './services/authentication.service';
import { User } from './models/user';
import { Angulartics2GoogleAnalytics } from 'angulartics2/ga';


declare let $: any;
declare const gtag: Function;


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent implements OnInit {

  navItems: any[] = [
    // { name: 'Home', route: 'home' },
    // { name: 'Resources', route: 'resources' },
    { name: 'Data Lab', route: 'data-extraction' },
    { name: 'Data Visualization', route: 'data-visualization' },
    // { name: 'MouseBytes Dashborad', route: 'mb-dashboard' },
    { name: 'Search', route: 'search-experiment' },
  ];

  navItemsTutorials: any[] = [

    { name: 'Guidelines', route: 'guideline' },
    { name: 'Video Tutorials', route: 'video-tutorial' },

  ];

  navItemsResources: any[] = [

    { name: 'Contact US', route: 'contact-us' },
    { name: 'Forms', route: 'forms' },
    { name: 'Terms of Service', route: 'terms' },
  ];

  navItemsIntegration: any[] = [
    { name: 'Imaging', route: 'imaging' },
    { name: 'Genomics', route: 'genomics' }
  ];

  navItemsPubscreen: any[] = [
    { name: 'Searching Publications', route: 'pubScreen-search' }
  ];

  public signedIn!: Observable<boolean>;

  name: string;
  isAdmin: boolean;
  isUser: boolean;
  isFullDataAccess: boolean;
  key: any;
  showFooter: boolean;

  constructor(
    public title: Title,
    private oAuthService: OAuthService,
    private authenticationService: AuthenticationService,
    private router: Router,
    private el: ElementRef,
    angulartics2GoogleAnalytics: Angulartics2GoogleAnalytics
  ) {
    this.name = '';
    this.isAdmin = false;
    this.isUser = false;
    this.isFullDataAccess = false;
    this.showFooter = false;

    if (this.oAuthService.hasValidAccessToken()) {
      this.authenticationService.init();

      // Strategy for refresh token through a scheduler.
      this.authenticationService.startupTokenRefresh();
    }

    this.router.events.subscribe((event) => {
      if (event instanceof NavigationEnd) {
        gtag('config', 'G-D1Q51EVX8L', { 'page_path': event.urlAfterRedirects });
      }
    });

  }

  ngOnInit() {
    this.title.setTitle('MouseBytes');

    this.signedIn = this.authenticationService.isSignedIn();

    this.authenticationService.userChanged().subscribe(
      (user: User) => {
        this.name = user.givenName;
        this.isAdmin = this.authenticationService.isInRole('administrator');
        this.isUser = this.authenticationService.isInRole('user');
        this.isFullDataAccess = this.authenticationService.isInRole('fulldataaccess');
      });


  }

  mouseEnter(div: string) {
    if (div == 'homelink') {
      $('#homeImage').css('visibility', 'visible');
    } else if (div == 'contactus') {
      $('#contactImage').css('visibility', 'visible');
    } else if (div == 'aboutus') {
      $('#aboutImage').css('visibility', 'visible');
    }
  }

  mouseLeave(div: string) {
    if (div == 'homelink') {
      $('#homeImage').css('visibility', 'hidden');
    } else if (div == 'contactus') {
      $('#contactImage').css('visibility', 'hidden');
    } else if (div == 'aboutus') {
      $('#aboutImage').css('visibility', 'hidden');
    }
  }

  signout(): void {
    this.authenticationService.signout();

    this.router.navigate(['/home']);
  }

  scrollTo(className: string): void {
    const elementList = document.querySelectorAll('.' + className);
    const element = elementList[0] as HTMLElement;
    element.scrollIntoView({ block: 'start', inline: 'nearest' }); // , behavior: 'smooth'
    // https://www.google.ca/search?rlz=1C1CHBF_enCA773CA773&ei=mjY0W7SqI6mZjwSm_alQ&q=scrollintoview+behavior+smooth+chrome+workaround&oq=scrollintoview+behavior+smooth+chrome+workaround&gs_l=psy-ab.3...4370.6235.0.6330.10.10.0.0.0.0.142.930.8j2.10.0....0...1c.1.64.psy-ab..0.9.852...33i22i29i30k1j33i160k1j33i21k1.0.pMohbacSZr0
  }

  navigateHomeAndScrollTo(element: any) {
    this.router.navigate(['/home']).then(() => {
      this.scrollTo(element);
    });
  }

  // @HostListener('document:keydown', ['$event'])
  // handleKeyboardEvent(event: KeyboardEvent) {
  //    let x = event.keyCode;
  //    if (x === 40) {
  //        console.log(2);
  //        this.scrollTo('about');
  //    }
  // }

  showHideFooter(routerUrl: string) {
    if (routerUrl.toUpperCase() === '' ||
            routerUrl.toUpperCase() === '/home'.toUpperCase() ||
            routerUrl.toUpperCase() === '/data-extraction'.toUpperCase() ||
            routerUrl.toUpperCase() === '/data-visualization'.toUpperCase() ||
            routerUrl.toUpperCase() === '/mb-dashboard'.toUpperCase() ||
            routerUrl.toUpperCase() === '/pubScreen-dashboard'.toUpperCase() ||
            routerUrl.toUpperCase().indexOf('/data-link'.toUpperCase()) != -1 ||
            routerUrl.toUpperCase() === '/guideline'.toUpperCase() ||
            routerUrl.toUpperCase() === '/imaging'.toUpperCase() ||
            routerUrl.toUpperCase() === '/genomics'.toUpperCase() ||
            routerUrl.toUpperCase() === '/video-tutorial'.toUpperCase() ||
            routerUrl.toUpperCase() === '/contact-us'.toUpperCase() ||
            routerUrl.toUpperCase() === '/forms'.toUpperCase() ||
            routerUrl.toUpperCase() === '/terms'.toUpperCase() ||
            routerUrl.toUpperCase() === '/account/signin'.toUpperCase() ||
            routerUrl.toUpperCase() === '/account/signup'.toUpperCase() ||
            routerUrl.toUpperCase() === '/search-experiment'.toUpperCase() ||
            routerUrl.toUpperCase() === '/pubScreen'.toUpperCase() ||
            routerUrl.toUpperCase() === '/pubScreen-queue'.toUpperCase() ||
            routerUrl.toUpperCase() === '/pubScreen-edit'.toUpperCase() ||
            routerUrl.toUpperCase() === '/comp-edit'.toUpperCase() ||
            // routerUrl.toUpperCase() === '/comp'.toUpperCase() ||
            routerUrl.toUpperCase() === '/comp-search'.toUpperCase()

    ) {
      this.showFooter = true;
    } else {
      this.showFooter = false;
    }

  }

  onActivate(event: any) {

    this.showHideFooter(this.router.url);
    // console.log(this.router.url);
    // console.log(event.constructor.name);
    // this.scrollTo("about");
    // if (e.constructor.name)==="login"{ // for example
    //    window.scroll(0, 0);
    // }
    // let scrollToTop = window.setInterval(() => {
    //    let pos = window.pageYOffset;
    //    if (pos > 0) {
    //        window.scrollTo(0, pos - 20); // how far to scroll on each step
    //    } else {
    //        window.clearInterval(scrollToTop);
    //    }
    // }, 16);
  }

}
