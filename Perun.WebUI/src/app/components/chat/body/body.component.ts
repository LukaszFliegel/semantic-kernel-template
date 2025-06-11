import { Component, Input, ViewChild, ElementRef } from '@angular/core';
import { Message } from '../../../models/message';
import { MessageComponent } from './message/message.component';

@Component({
  selector: 'app-body',
  standalone: true,
  imports: [MessageComponent],
  templateUrl: './body.component.html',
  styleUrl: './body.component.scss'
})
export class BodyComponent {
  @Input({ required: true }) history: Message[] = [];
  @ViewChild('scrollContainer') private scrollContainer!: ElementRef;

  scrollToBottom(): void {
    try {
      this.scrollContainer.nativeElement.scrollTop = this.scrollContainer.nativeElement.scrollHeight;
    } catch(err) { }
  }
}
