import { Component, OnInit } from '@angular/core';
import { faEnvelope } from '@fortawesome/free-solid-svg-icons'
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';

@Component({
  selector: 'app-contact-us',
  templateUrl: './contact-us.component.html',
  styleUrls: ['./contact-us.component.scss']
  })
export class ContactUsComponent implements OnInit {
    faEnvelope = faEnvelope;
  constructor() { }

  ngOnInit() {
  }

}
