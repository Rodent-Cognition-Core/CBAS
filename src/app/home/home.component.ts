import { Component } from '@angular/core';
// import { SharedModule } from '../shared/shared.module';
// import { CarouselModule } from 'ngx-owl-carousel-o';
// import { Router, NavigationEnd } from '@angular/router';
import { ScrollService } from '../shared/scroll.service';
import { AnimalService } from '../services/animal.service';

declare let $: any;

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
  providers: [AnimalService]
  })
export class HomeComponent {
  showItem: boolean;

  images: any;

  mouseCounter: number;

  carouselOptions: any = {
    items: 1, dots: false, nav: true, loop: true,
    navText: ['<i class=\'fa fa-chevron-circle-left\' style=\'color:  #795D9C\'></i>',
      '<i class=\'fa fa-chevron-circle-right\' style=\'color:   #795D9C\'></i>']
  };

  constructor(
    private animalService: AnimalService,
    private scrollService: ScrollService,
  ) {
    this.showItem = false;
    this.mouseCounter = 0;

  }

  ngAfterViewInit() {
    this.scrollService.scrollToTop();
  }

  ngOnInit() {


    this.images = [
    ];

    // this.mouseCounter = 1200;
    this.animalService.getCountOfAnimals().subscribe(data => {
      this.mouseCounter = data;

    });
    // this.mouseCounter = this.animalService.getCountOfAnimals();

  }

  ngAfterViewChecked() {
    setTimeout(() => {
      this.showItem = true;
    });
  }


}





