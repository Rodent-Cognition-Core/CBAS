import { Component } from '@angular/core';
import { faEnvelope } from '@fortawesome/free-solid-svg-icons'

@Component({
  selector: 'app-contact-us',
  templateUrl: './contact-us.component.html',
  styleUrls: ['./contact-us.component.scss']
})
export class ContactUsComponent{
  faEnvelope = faEnvelope;
  constructor() { }

}
